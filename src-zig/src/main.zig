const std = @import("std");
const glfw = @import("mach-glfw");
const gl = @import("gl");
const math = @import("mach").math;
const c = @import("c.zig");

const _engine = @import("engine.zig");
const Mesh = _engine.Mesh;
const Shader = _engine.Shader;
const Engine = _engine.Engine;
const Vertex = _engine.Vertex;
const GameObject = _engine.GameObject;
const Texture = _engine.Texture;
const Material = _engine.Material;

const Color = @import("color.zig");

const Shapes = @import("shapes.zig");

pub fn main() !void {
    var engine = Engine{};
    try engine.init(.{ .width = 1920, .height = 1080 });
    defer engine.deinit();

    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    defer _ = gpa.deinit();
    const alloc = gpa.allocator();

    // Cube
    var sphereMesh = Mesh.init(alloc);
    // try Shapes.sphere(&sphereMesh, 64, 32, 1);
    try Shapes.quad(&sphereMesh);
    try sphereMesh.create();
    defer sphereMesh.deinit();

    var shader = Shader{
        .vertSource = @embedFile("./shaders/vert.glsl"),
        .fragSource = @embedFile("./shaders/two_textures.glsl"),
    };
    try shader.compile();
    defer shader.deinit();

    var brickTexture = Texture{};
    try brickTexture.load("res/brick.png");
    defer brickTexture.deinit();
    brickTexture.log();
    brickTexture.create();

    var planeTexture = Texture{};
    try planeTexture.load("res/plane.png");
    defer planeTexture.deinit();
    planeTexture.log();
    planeTexture.create();

    var brickMaterial = Material{ .shader = &shader };
    // try brickMaterial.addProp("_Color", Color.red.toVec4());
    try brickMaterial.addProp("_Texture", &brickTexture);
    try brickMaterial.addProp("_Texture2", &planeTexture);

    var planeMaterial = Material{ .shader = &shader };
    // try planeMaterial.addProp("_Color", Color.blue.toVec4());
    try planeMaterial.addProp("_Texture", &planeTexture);
    try planeMaterial.addProp("_Texture2", &brickTexture);

    var motion = math.vec3(0, 0, 0);
    var camOffset = math.vec3(4, 0, 10);

    var wireframe = false;

    engine.createScene();
    var sphereGO = try engine.scene.?.addGameObject(&sphereMesh, &brickMaterial);
    var sphereGO2 = try engine.scene.?.addGameObject(&sphereMesh, &planeMaterial);

    var pcg = std.rand.Pcg.init(345);

    for (0..200) |i| {
        _ = i;
        if (pcg.random().boolean()) {
            _ = try engine.scene.?.addGameObject(&sphereMesh, &brickMaterial);
        } else {
            _ = try engine.scene.?.addGameObject(&sphereMesh, &planeMaterial);
        }
    }

    var lastFrameTime = glfw.getTime();

    const color = Color.init(0, 0, 0, 0);
    _ = color;

    while (engine.isRunning()) {
        var dt: f32 = @floatCast(glfw.getTime() - lastFrameTime);
        lastFrameTime = glfw.getTime();

        const speed = dt;

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

            Engine.setWireframe(wireframe);
        }

        var camOffsetMat = math.Mat4x4.translate(camOffset);
        engine.camera.viewMatrix = math.Mat4x4.ident.mul(&camOffsetMat);

        shader.bind();

        motion.v[0] = @floatCast(@sin(glfw.getTime()));
        motion.v[1] = @floatCast(@cos(glfw.getTime()));

        var modelMatrix = math.Mat4x4.ident.mul(&math.Mat4x4.translate(motion));

        try shader.setUniformByName("_P", engine.camera.projectionMatrix);
        try shader.setUniformByName("_V", engine.camera.viewMatrix);
        // try shader.setUniformByName("_Color", Color.chartreuse.toVec4());
        // try shader.setUniformByName("_Texture", @as(i32, @intCast(id)));

        sphereGO.transform.local2world = modelMatrix;
        sphereGO2.transform.local2world = modelMatrix.mul(&math.Mat4x4.translate(math.vec3(5, 2, 0)));

        for (engine.scene.?.gameObjects.slice()) |*object| {
            const xMotion = std.math.lerp(-1, 1, pcg.random().float(f32)) * 0.1;
            const yMotion = std.math.lerp(-1, 1, pcg.random().float(f32)) * 0.1;
            const zMotion = std.math.lerp(-1, 1, pcg.random().float(f32)) * 0.1;

            const motionVec = math.vec3(xMotion, yMotion, zMotion);

            object.transform.local2world = object.transform.local2world.mul(&math.Mat4x4.translate(motionVec));
        }

        if (engine.scene) |scene| {
            try scene.render();
        }
    }
}

test "color" {
    _ = Color;
}
