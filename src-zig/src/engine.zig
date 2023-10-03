const std = @import("std");
const glfw = @import("mach-glfw");
const gl = @import("gl");
const math = @import("mach").math;

pub const WindowProps = struct {
    width: u32 = 1080,
    height: u32 = 720,
    title: [:0]const u8 = "Phoenix",
    vsync: bool = true,
};

pub const Engine = struct {
    window: ?glfw.Window = null,
    camera: Camera = .{},

    const Self = @This();

    /// Default GLFW error handling callback
    fn errorCallback(error_code: glfw.ErrorCode, description: [:0]const u8) void {
        std.log.err("glfw: {}: {s}\n", .{ error_code, description });
    }

    fn glGetProcAddress(p: glfw.GLProc, proc: [:0]const u8) ?gl.FunctionPointer {
        _ = p;
        return glfw.getProcAddress(proc);
    }

    pub fn init(self: *Self, windowProps: WindowProps) !void {
        glfw.setErrorCallback(errorCallback);
        if (!glfw.init(.{})) {
            std.log.err("failed to initialize GLFW: {?s}", .{glfw.getErrorString()});
            std.process.exit(1);
        }

        self.window = glfw.Window.create(
            windowProps.width,
            windowProps.height,
            windowProps.title,
            null,
            null,
            .{},
        ) orelse {
            std.log.err("failed to create GLFW window: {?s}", .{glfw.getErrorString()});
            std.process.exit(1);
        };

        glfw.makeContextCurrent(self.window);

        const proc: glfw.GLProc = undefined;
        try gl.load(proc, glGetProcAddress);

        self.camera.engine = self;
        self.camera.updateProjectionMatrix();
    }

    pub fn deinit(self: Self) void {
        if (self.window) |window| {
            window.destroy();
        }
        glfw.terminate();
    }

    pub fn isRunning(self: Self) bool {
        self.window.?.swapBuffers();

        glfw.pollEvents();

        gl.clearColor(0, 1, 0, 1);
        gl.clear(gl.COLOR_BUFFER_BIT);

        return !self.window.?.shouldClose();
    }

    pub fn keyPressed(self: Self, key: glfw.Key) bool {
        return self.window.?.getKey(key) == glfw.Action.press;
    }
};

pub const Camera = struct {
    projectionMatrix: math.Mat4x4 = math.Mat4x4.ident,
    viewMatrix: math.Mat4x4 = math.Mat4x4.ident,

    nearPlane: f32 = -1,
    farPlane: f32 = 1000,
    fov: f32 = 75,
    aspectRatio: f32 = 1,

    engine: *Engine = undefined,

    pub fn updateProjectionMatrix(self: *Camera) void {
        const size = self.engine.window.?.getSize();
        self.aspectRatio = @as(f32, @floatFromInt(size.width)) / @as(f32, @floatFromInt(size.height));

        self.projectionMatrix = math.Mat4x4.perspective(
            math.degreesToRadians(f32, self.fov),
            self.aspectRatio,
            self.nearPlane,
            self.farPlane,
        );
    }
};

pub const Mesh = struct {
    vertices: std.ArrayList(f32),
    indices: std.ArrayList(u32),

    vao: u32 = undefined,
    vbo: u32 = undefined,
    ibo: u32 = undefined,

    const Self = @This();

    pub fn init(alloc: std.mem.Allocator) Mesh {
        return .{
            .vertices = std.ArrayList(f32).init(alloc),
            .indices = std.ArrayList(u32).init(alloc),
        };
    }

    pub fn create(self: *Self) void {
        // VAO VBO IBO
        gl.genVertexArrays(1, &self.vao);
        gl.genBuffers(1, &self.vbo);
        gl.genBuffers(1, &self.ibo);

        gl.bindVertexArray(self.vao);

        gl.bindBuffer(gl.ARRAY_BUFFER, self.vbo);
        gl.bufferData(gl.ARRAY_BUFFER, @intCast(self.vertices.items.len * @sizeOf(f32)), self.vertices.items[0..].ptr, gl.STATIC_DRAW);

        gl.vertexAttribPointer(0, 3, gl.FLOAT, gl.FALSE, 3 * @sizeOf(f32), null);
        gl.enableVertexAttribArray(0);

        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, self.ibo);
        gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, @intCast(self.indices.items.len * @sizeOf(u32)), self.indices.items[0..].ptr, gl.STATIC_DRAW);

        gl.bindBuffer(gl.ARRAY_BUFFER, 0);
        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, 0);
        gl.bindVertexArray(0);
    }

    pub fn bind(self: Self) void {
        gl.bindVertexArray(self.vao);
        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, self.ibo);
        gl.drawElements(gl.TRIANGLES, @intCast(self.indices.items.len), gl.UNSIGNED_INT, null);
    }

    pub fn deinit(self: Self) void {
        gl.deleteVertexArrays(1, &self.vao);
        gl.deleteBuffers(1, &self.vbo);
        gl.deleteBuffers(1, &self.ibo);

        self.vertices.deinit();
        self.indices.deinit();
    }
};

pub const Shader = struct {
    program: u32 = 0,
    vertSource: []const u8,
    fragSource: []const u8,

    const Self = @This();

    const Error = error{InvalidUniformName};

    pub fn compile(self: *Self) void {
        var vertShader: u32 = gl.createShader(gl.VERTEX_SHADER);
        gl.shaderSource(vertShader, 1, &self.vertSource.ptr, null);
        gl.compileShader(vertShader);

        var fragShader: u32 = gl.createShader(gl.FRAGMENT_SHADER);
        gl.shaderSource(fragShader, 1, &self.fragSource.ptr, null);
        gl.compileShader(fragShader);

        self.program = gl.createProgram();
        gl.attachShader(self.program, vertShader);
        gl.attachShader(self.program, fragShader);
        gl.linkProgram(self.program);

        gl.deleteShader(vertShader);
        gl.deleteShader(fragShader);
    }

    pub fn bind(self: Self) void {
        gl.useProgram(self.program);
    }

    pub fn deinit(self: Self) void {
        gl.deleteProgram(self.program);
    }

    pub fn setUniform(location: i32, value: anytype) void {
        comptime {
            const T = @TypeOf(value);

            if (T != i32 and T != f32 and T != math.Vec2 and T != math.Vec3 and T != math.Vec4 and T != math.Mat4x4) {
                @compileError("Uniform with type of " ++ @typeName(T) ++ " is not supported");
            }
        }

        switch (@TypeOf(value)) {
            inline i32 => gl.uniform1i(location, value),
            inline f32 => gl.uniform1f(location, value),
            inline math.Vec2 => gl.uniform2fv(location, 1, &value.v[0]),
            inline math.Vec3 => gl.uniform3fv(location, 1, &value.v[0]),
            inline math.Vec4 => gl.uniform4fv(location, 1, &value.v[0]),
            inline math.Mat4x4 => gl.uniformMatrix4fv(location, 1, gl.FALSE, &value.v[0].v[0]),
            inline else => unreachable,
        }
    }

    pub fn setUniformByName(self: Self, name: [:0]const u8, value: anytype) !void {
        const location = gl.getUniformLocation(self.program, name);

        if (location < 0)
            return error.InvalidUniformName;

        setUniform(location, value);
    }
};
