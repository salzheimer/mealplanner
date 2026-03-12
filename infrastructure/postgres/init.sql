-- PostgreSQL initialization script for Meal Planner
-- Derived from docs/meal_planning_datamodel.drawio

-- extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- enum types
CREATE TYPE visibility_enum AS ENUM ('private','shared','group');
CREATE TYPE meal_type_enum AS ENUM ('breakfast','lunch','dinner','snack');
CREATE TYPE item_type_enum AS ENUM ('recipe','homemade','store_bought');
CREATE TYPE status_enum AS ENUM ('confirmed','pending','unknown');

-- users table (referenced by other tables)
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username TEXT NOT NULL
    -- add additional fields as needed by AuthService
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

-- plans
CREATE TABLE IF NOT EXISTS plan (
    id SERIAL PRIMARY KEY,
    start_date DATE,
    end_date DATE,
    name TEXT
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

