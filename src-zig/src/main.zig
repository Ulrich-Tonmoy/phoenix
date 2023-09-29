const std = @import("std");
const glfw = @import("mach-glfw");
const gl = @import("gl");

fn glGetProcAddress(p: glfw.GLProc, proc: [:0]const u8) ?gl.FunctionPointer {
    _ = p;
    return glfw.getProcAddress(proc);
}

/// Default GLFW error handling callback
fn errorCallback(error_code: glfw.ErrorCode, description: [:0]const u8) void {
    std.log.err("glfw: {}: {s}\n", .{ error_code, description });
}

const Mesh = struct {
    vertices: [32]f32 = [1]f32{0} ** 32,
    indices: [32]u32 = [1]u32{0} ** 32,

    vertexCount: isize = 0,
    indexCount: isize = 0,

    vao: u32 = undefined,
    vbo: u32 = undefined,
    ibo: u32 = undefined,

    const Self = @This();

    fn create(self: *Self) void {
        // VAO VBO IBO
        gl.genVertexArrays(1, &self.vao);
        gl.genBuffers(1, &self.vbo);
        gl.genBuffers(1, &self.ibo);

        gl.bindVertexArray(self.vao);

        gl.bindBuffer(gl.ARRAY_BUFFER, self.vbo);
        gl.bufferData(gl.ARRAY_BUFFER, self.vertexCount * @sizeOf(f32), self.vertices[0..].ptr, gl.STATIC_DRAW);

        gl.vertexAttribPointer(0, 3, gl.FLOAT, gl.FALSE, 3 * @sizeOf(f32), null);
        gl.enableVertexAttribArray(0);

        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, self.ibo);
        gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, self.indexCount * @sizeOf(u32), self.indices[0..].ptr, gl.STATIC_DRAW);

        gl.bindBuffer(gl.ARRAY_BUFFER, 0);
        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, 0);
        gl.bindVertexArray(0);
    }

    fn bind(self: Self) void {
        gl.bindVertexArray(self.vao);
        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, self.ibo);
        gl.drawElements(gl.TRIANGLES, @intCast(self.indexCount), gl.UNSIGNED_INT, null);
    }

    fn deinit(self: Self) void {
        gl.deleteVertexArrays(1, &self.vao);
        gl.deleteBuffers(1, &self.vbo);
        gl.deleteBuffers(1, &self.ibo);
    }
};

const Shader = struct {
    program: u32 = 0,

    const Self = @This();

    fn compile(self: *Self) void {
        const vertShaderSource: []const u8 = @embedFile("vert.glsl");
        const fragShaderSource: []const u8 = @embedFile("frag.glsl");

        var vertShader: u32 = gl.createShader(gl.VERTEX_SHADER);
        gl.shaderSource(vertShader, 1, &vertShaderSource.ptr, null);
        gl.compileShader(vertShader);

        var fragShader: u32 = gl.createShader(gl.FRAGMENT_SHADER);
        gl.shaderSource(fragShader, 1, &fragShaderSource.ptr, null);
        gl.compileShader(fragShader);

        self.program = gl.createProgram();
        gl.attachShader(self.program, vertShader);
        gl.attachShader(self.program, fragShader);
        gl.linkProgram(self.program);

        gl.deleteShader(vertShader);
        gl.deleteShader(fragShader);
    }

    fn bind(self: Self) void {
        gl.useProgram(self.program);
    }

    fn deinit(self: Self) void {
        gl.deleteProgram(self.program);
    }
};

pub fn main() !void {
    glfw.setErrorCallback(errorCallback);
    if (!glfw.init(.{})) {
        std.log.err("failed to initialize GLFW: {?s}", .{glfw.getErrorString()});
        std.process.exit(1);
    }
    defer glfw.terminate();

    var window = glfw.Window.create(1080, 720, "Phoenix", null, null, .{}) orelse {
        std.log.err("failed to create GLFW window: {?s}", .{glfw.getErrorString()});
        std.process.exit(1);
    };
    defer window.destroy();

    glfw.makeContextCurrent(window);

    const proc: glfw.GLProc = undefined;
    try gl.load(proc, glGetProcAddress);

    // Data
    const vertices = [_]f32{
        -0.5, -0.5, 0,
        0.5,  -0.5, 0,
        0,    0.5,  0,
    };

    const indices = [_]u32{
        0, 1, 2,
    };

    var mesh = Mesh{};

    @memcpy(mesh.vertices[0..vertices.len], vertices[0..]);
    mesh.vertexCount = vertices.len;

    @memcpy(mesh.indices[0..indices.len], indices[0..]);
    mesh.indexCount = indices.len;

    mesh.create();
    defer mesh.deinit();

    var shader = Shader{};
    shader.compile();
    defer shader.deinit();

    while (!window.shouldClose()) {
        window.swapBuffers();

        gl.clearColor(0, 1, 0, 1);
        gl.clear(gl.COLOR_BUFFER_BIT);

        shader.bind();
        mesh.bind();

        glfw.pollEvents();
    }
}
