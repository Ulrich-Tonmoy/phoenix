const std = @import("std");
const glfw = @import("mach-glfw");
const gl = @import("gl");
const math = @import("mach").math;
const c = @import("c.zig");
const gltf = @import("zcgltf.zig");

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

    // Sphere
    var sphereMesh = Mesh.init(alloc);
    defer sphereMesh.deinit();
    try Shapes.sphere(&sphereMesh, 32, 16, 1);
    try sphereMesh.create();

    // Quad
    var quadMesh = Mesh.init(alloc);
    try Shapes.quad(&quadMesh);

    for (quadMesh.vertices.items) |*v| {
        v.position = v.position.add(&math.vec3(2, 0, 0));
    }

    try Shapes.quad(&quadMesh);
    try quadMesh.create();
    defer quadMesh.deinit();

    var shader = Shader{
        .vertSource = @embedFile("./shaders/vert.glsl"),
        .fragSource = @embedFile("./shaders/frag.glsl"),
    };
    try shader.compile();
    defer shader.deinit();

    var brickTexture = Texture{};
    try brickTexture.load("res/brick.png");
    defer brickTexture.deinit();
    brickTexture.log();
    try brickTexture.create();

    var planeTexture = Texture{};
    try planeTexture.load("res/uv_checker.png");
    defer planeTexture.deinit();
    planeTexture.log();
    try planeTexture.create();

    var brickMaterial = Material{ .shader = &shader };
    try brickMaterial.addProp("_Color", Color.white);
    try brickMaterial.addProp("_Texture", &brickTexture);

    var planeMaterial = Material{ .shader = &shader };
    try planeMaterial.addProp("_Color", Color.white);
    try planeMaterial.addProp("_Texture", &planeTexture);

    var motion = math.vec3(0, 0, 0);
    var camOffset = math.vec3(4, 0, 10);

    var wireframe = false;

    engine.createScene();
    var sphereGO = try engine.scene.?.addGameObject(&sphereMesh, &brickMaterial);
    var sphereGO2 = try engine.scene.?.addGameObject(&sphereMesh, &planeMaterial);

    // GLTF
    var data = try gltf.parseFile(.{}, "res/3d/testcube.gltf");
    try gltf.loadBuffers(.{}, data, "res/3d/testcube.gltf");

    for (data.meshes.?[0..data.meshes_count]) |mesh| {
        for (mesh.primitives[0..mesh.primitives_count]) |primitive| {
            var gameMesh = Mesh.init(alloc);

            for (primitive.attributes[0..primitive.attributes_count]) |attribute| {
                var name = std.mem.sliceTo(attribute.name.?, 0);
                if (std.mem.eql(u8, name, "POSITION")) {
                    std.log.info("Found position!", .{});

                    var accessor = attribute.data;
                    const vertexCount = accessor.count;
                    try gameMesh.vertices.ensureTotalCapacity(vertexCount);
                    var buffer = accessor.buffer_view.?.buffer;

                    var vertData = @as([*]@Vector(3, f32), @ptrCast(@alignCast(buffer.data.?)))[0..vertexCount];

                    for (0..vertexCount) |vi| {
                        var vec = vertData[vi];
                        std.log.info("vec: {}", .{vec});
                        try gameMesh.vertices.append(Vertex{ .position = .{ .v = vec } });
                    }
                }
            }
        }
    }

    var lastFrameTime = glfw.getTime();

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

        motion.v[0] = @floatCast(@sin(glfw.getTime()));
        motion.v[1] = @floatCast(@cos(glfw.getTime()));

        var modelMatrix = math.Mat4x4.ident.mul(&math.Mat4x4.translate(motion));

        shader.bind();
        try shader.setUniformByName("_P", engine.camera.projectionMatrix);
        try shader.setUniformByName("_V", engine.camera.viewMatrix);

        sphereGO.transform.local2world = modelMatrix;
        sphereGO2.transform.local2world = modelMatrix.mul(&math.Mat4x4.translate(math.vec3(5, 2, 0)));

        if (engine.scene) |scene| try scene.render();
    }
}

test "color" {
    _ = Color;
}
