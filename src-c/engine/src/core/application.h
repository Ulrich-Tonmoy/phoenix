#pragma once

#include "defines.h"

struct game;

// Application configuration.
typedef struct application_config {
    // Window starting position x axis, if applicable.
    i16 start_pos_x;

    // Window starting position y axis, if applicable.
    i16 start_pos_y;

    // Window starting width, if applicable.
    i16 start_width;

    // Window starting height, if applicable.
    i16 start_height;

    // The application name used in windowing, if applicable.
    char* name;
} application_config;

PAPI b8 application_create(struct game* game_inst);

PAPI b8 application_run();