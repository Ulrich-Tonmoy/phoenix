const std = @import("std");
const glfw = @import("mach-glfw");
const math = @import("mach").math;

const _engine = @import("engine.zig");
const Mesh = _engine.Mesh;
const Shader = _engine.Shader;
const Engine = _engine.Engine;

pub fn main() !void {
    var engine = Engine{};
    try engine.init();
    defer engine.deinit();

    // Data
    const vertices = [_]f32{
        -0.5, -0.5, 0,
        0.5,  -0.5, 0,
        0,    0.5,  0,
    };

    const indices = [_]u32{
        0, 1, 2,
    };

    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    defer _ = gpa.deinit();
    const alloc = gpa.allocator();

    var mesh = Mesh.init(alloc);
    try mesh.vertices.appendSlice(vertices[0..]);
    try mesh.indices.appendSlice(indices[0..]);
    mesh.create();
    defer mesh.deinit();

    var mesh2 = Mesh.init(alloc);
    try mesh2.vertices.appendSlice(&.{
        0, 0, 0,
        1, 0, 0,
        0, 1, 0,
        1, 1, 0,
        0, 0, 1,
        1, 0, 1,
        0, 1, 1,
        1, 1, 1,
    });
    try mesh2.indices.appendSlice(&.{
        // front
        0, 1, 2,
        2, 3, 0,
        // right
        1, 5, 6,
        6, 2, 1,
        // back
        7, 6, 5,
        5, 4, 7,
        // left
        4, 0, 3,
        3, 7, 4,
        // bottom
        4, 5, 1,
        1, 0, 4,
        // top
        3, 2, 6,
        6, 7, 3,
    });
    mesh2.create();
    defer mesh2.deinit();

    var shader = Shader{
        .vertSource = @embedFile("vert.glsl"),
        .fragSource = @embedFile("frag.glsl"),
    };
    shader.compile();
    defer shader.deinit();

    var motion = math.vec3(0, 0, 0);
    var camOffset = math.vec3(4, 0, 10);

    while (engine.isRunning()) {
        const speed = 0.001;

        if (engine.keyPressed(.w) or engine.keyPressed(.up)) {
            camOffset.v[2] -= speed;
        } else if (engine.keyPressed(.s) or engine.keyPressed(.down)) {
            camOffset.v[2] += speed;
        }
        if (engine.keyPressed(.a) or engine.keyPressed(.left)) {
            camOffset.v[0] -= speed;
        } else if (engine.keyPressed(.d) or engine.keyPressed(.right)) {
            camOffset.v[0] += speed;
        }

        if (engine.keyPressed(.c)) {
            engine.camera.nearPlane += 0.001;
            engine.camera.updateProjectionMatrix();
        } else if (engine.keyPressed(.x)) {
            engine.camera.nearPlane -= 0.001;
            engine.camera.updateProjectionMatrix();
        }

        var camOffsetMat = math.Mat4x4.translate(camOffset);
        engine.camera.viewMatrix = math.Mat4x4.ident.mul(&camOffsetMat);

        shader.bind();
        motion.v[0] = @floatCast(@sin(glfw.getTime()));
        Shader.setVec3(0, motion);
        Shader.setMatrix(1, engine.camera.projectionMatrix);
        Shader.setMatrix(2, engine.camera.viewMatrix);

        mesh.bind();
        mesh2.bind();
    }
}
