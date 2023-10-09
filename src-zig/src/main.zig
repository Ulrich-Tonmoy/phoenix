const std = @import("std");
const glfw = @import("mach-glfw");
const gl = @import("gl");
const math = @import("mach").math;

const _engine = @import("engine.zig");
const Mesh = _engine.Mesh;
const Shader = _engine.Shader;
const Engine = _engine.Engine;
const Vertex = _engine.Vertex;
const GameObject = _engine.GameObject;

const Shapes = @import("shapes.zig");

pub fn main() !void {
    var engine = Engine{};
    try engine.init(.{});
    defer engine.deinit();

    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    defer _ = gpa.deinit();
    const alloc = gpa.allocator();

    // Cube
    var mesh = Mesh.init(alloc);
    try Shapes.sphere(&mesh, 64, 32, 1);
    try mesh.create();
    defer mesh.deinit();

    var shader = Shader{
        .vertSource = @embedFile("./shaders/vert.glsl"),
        .fragSource = @embedFile("./shaders/frag.glsl"),
    };
    try shader.compile();
    defer shader.deinit();

    var sphereGO = GameObject{
        .mesh = &mesh,
        .shader = &shader,
    };
    var sphereGO2 = GameObject{
        .mesh = &mesh,
        .shader = &shader,
    };

    var motion = math.vec3(0, 0, 0);
    var camOffset = math.vec3(4, 0, 10);

    var wireframe = false;
    while (engine.isRunning()) {
        const speed = 0.001;

        if (engine.input.keyPressed(.w) or engine.input.keyPressed(.up)) {
            camOffset.v[2] -= speed;
        } else if (engine.input.keyPressed(.s) or engine.input.keyPressed(.down)) {
            camOffset.v[2] += speed;
        }
        if (engine.input.keyPressed(.a) or engine.input.keyPressed(.left)) {
            camOffset.v[0] += speed;
        } else if (engine.input.keyPressed(.d) or engine.input.keyPressed(.right)) {
            camOffset.v[0] -= speed;
        }

        if (engine.input.keyPressed(.c)) {
            engine.camera.nearPlane += 0.001;
            engine.camera.updateProjectionMatrix();
        } else if (engine.input.keyPressed(.x)) {
            engine.camera.nearPlane -= 0.001;
            engine.camera.updateProjectionMatrix();
        }

        if (engine.input.keyDown(.q)) {
            wireframe = !wireframe;

            if (wireframe) {
                gl.polygonMode(gl.FRONT_AND_BACK, gl.LINE);
            } else {
                gl.polygonMode(gl.FRONT, gl.FILL);
            }
        }

        var camOffsetMat = math.Mat4x4.translate(camOffset);
        engine.camera.viewMatrix = math.Mat4x4.ident.mul(&camOffsetMat);

        shader.bind();

        motion.v[0] = @floatCast(@sin(glfw.getTime()));
        motion.v[1] = @floatCast(@cos(glfw.getTime()));

        var modelMatrix = math.Mat4x4.ident.mul(&math.Mat4x4.translate(motion));

        Shader.setUniform(1, engine.camera.projectionMatrix);
        Shader.setUniform(2, engine.camera.viewMatrix);

        sphereGO.transform.local2world = modelMatrix;
        sphereGO2.transform.local2world = modelMatrix.mul(&math.Mat4x4.translate(math.vec3(5, 2, 0)));

        try sphereGO.render();
        try sphereGO2.render();
    }
}
