const std = @import("std");
const glfw = @import("mach-glfw");
const gl = @import("gl");
const math = @import("mach").math;

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

    pub fn init(self: *Self) !void {
        glfw.setErrorCallback(errorCallback);
        if (!glfw.init(.{})) {
            std.log.err("failed to initialize GLFW: {?s}", .{glfw.getErrorString()});
            std.process.exit(1);
        }

        self.window = glfw.Window.create(1080, 720, "Phoenix", null, null, .{}) orelse {
            std.log.err("failed to create GLFW window: {?s}", .{glfw.getErrorString()});
            std.process.exit(1);
        };

        glfw.makeContextCurrent(self.window);

        const proc: glfw.GLProc = undefined;
        try gl.load(proc, glGetProcAddress);

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
    aspectRation: f32 = 1,

    pub fn updateProjectionMatrix(self: *Camera) void {
        self.projectionMatrix = math.Mat4x4.perspective(
            math.degreesToRadians(f32, self.fov),
            self.aspectRation,
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

    pub fn setVec3(uniformLocation: i32, vec: math.Vec3) void {
        gl.uniform3fv(uniformLocation, 1, &vec.v[0]);
    }

    pub fn setMatrix(uniformLocation: i32, matrix: math.Mat4x4) void {
        gl.uniformMatrix4fv(uniformLocation, 1, gl.FALSE, &matrix.v[0].v[0]);
    }
};
