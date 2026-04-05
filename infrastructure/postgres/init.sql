-- PostgreSQL initialization script for Meal Planner
-- Derived from docs/meal_planning_datamodel.drawio

-- extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- enum types
CREATE TYPE visibility_enum AS ENUM ('private', 'shared', 'group');
CREATE TYPE meal_type_enum AS ENUM ('breakfast', 'lunch', 'dinner', 'snack');
CREATE TYPE item_type_enum AS ENUM ('recipe', 'homemade', 'store_bought');
CREATE TYPE status_enum AS ENUM ('confirmed', 'pending', 'unknown');
CREATE TYPE group_member_role_enum AS ENUM ('owner', 'member');
CREATE TYPE group_member_status_enum AS ENUM ('pending', 'active', 'removed');
CREATE TYPE permission_enum AS ENUM ('view', 'edit');
CREATE TYPE client_type_enum AS ENUM ('web', 'mobile', 'api');

-- users
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    display_name TEXT NOT NULL,
    email TEXT,
    email_verified BOOLEAN NOT NULL DEFAULT FALSE,
    email_verified_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP WITH TIME ZONE,
    auth0_id TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    failed_login_attempts INT NOT NULL DEFAULT 0,
    locked_until TIMESTAMP WITH TIME ZONE,
    terms_accepted_at TIMESTAMP WITH TIME ZONE,
    terms_version TEXT,
    security_stamp TEXT
);

-- groups
CREATE TABLE IF NOT EXISTS "group" (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    created_by_user_id INT REFERENCES users(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- group members
CREATE TABLE IF NOT EXISTS group_member (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    group_id INT NOT NULL REFERENCES "group"(id),
    role group_member_role_enum NOT NULL DEFAULT 'member',
    invited_by_user_id INT REFERENCES users(id),
    invited_at TIMESTAMP WITH TIME ZONE,
    joined_at TIMESTAMP WITH TIME ZONE,
    status group_member_status_enum NOT NULL DEFAULT 'pending'
);

-- user credentials
CREATE TABLE IF NOT EXISTS user_credential (
    id UUID PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    password_hash TEXT NOT NULL,
    hash_algorithm TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- password resets
CREATE TABLE IF NOT EXISTS password_reset (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    token_hash TEXT NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    used_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- sessions
CREATE TABLE IF NOT EXISTS session (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    token_hash TEXT NOT NULL,
    client_type client_type_enum NOT NULL,
    device_info TEXT,
    ip_address TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    last_used_at TIMESTAMP WITH TIME ZONE,
    revoked_at TIMESTAMP WITH TIME ZONE,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL
);

-- audit log
CREATE TABLE IF NOT EXISTS audit_log (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(id),
    session_id INT REFERENCES session(id),
    client_type TEXT,
    action TEXT NOT NULL,
    ip_address TEXT,
    resource_type TEXT,
    resource_id TEXT,
    metadata JSONB,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- recipes
CREATE TABLE IF NOT EXISTS recipe (
    id SERIAL PRIMARY KEY,
    description TEXT,
    notes TEXT,
    name TEXT,
    ranking INT,
    original_source TEXT,
    cook_time TIME,
    prep_time TIME,
    servings INT,
    owner_user_id INT REFERENCES users(id),
    visibility visibility_enum
);

-- recipe instructions
CREATE TABLE IF NOT EXISTS recipe_instructions (
    id SERIAL PRIMARY KEY,
    recipe_id INT NOT NULL REFERENCES recipe(id),
    description TEXT,
    step_number INT,
    note TEXT
);

-- recipe ingredients
CREATE TABLE IF NOT EXISTS recipe_ingredients (
    id SERIAL PRIMARY KEY,
    name TEXT,
    amount NUMERIC,
    measurement_type TEXT,
    recipe_id INT REFERENCES recipe(id)
);

-- recipe shares
CREATE TABLE IF NOT EXISTS recipe_share (
    id SERIAL PRIMARY KEY,
    recipe_id INT NOT NULL REFERENCES recipe(id),
    shared_with_user_id INT REFERENCES users(id),
    shared_with_group_id INT REFERENCES "group"(id),
    permission permission_enum NOT NULL DEFAULT 'view',
    shared_by_user_id INT REFERENCES users(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP WITH TIME ZONE
);

-- plans
CREATE TABLE IF NOT EXISTS plan (
    id SERIAL PRIMARY KEY,
    start_date DATE,
    end_date DATE,
    name TEXT,
    group_id INT REFERENCES "group"(id)
);

-- meals
CREATE TABLE IF NOT EXISTS meal (
    id SERIAL PRIMARY KEY,
    meal_type meal_type_enum,
    is_multi_day_meal BOOLEAN,
    date DATE,
    end_date DATE,
    plan_id INT REFERENCES plan(id)
);

-- meal items
CREATE TABLE IF NOT EXISTS meal_items (
    id SERIAL PRIMARY KEY,
    name TEXT,
    meal_id INT REFERENCES meal(id),
    recipe_id INT REFERENCES recipe(id),
    item_type item_type_enum,
    assigned_to_guest_name TEXT,
    assigned_to_user INT REFERENCES users(id),
    status status_enum,
    notes TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);
