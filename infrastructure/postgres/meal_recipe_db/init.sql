-- PostgreSQL initialization script for Meal Recipe Sevice DB

-- extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- lookup tables (replace enum types)

CREATE TABLE IF NOT EXISTS meal_types (
    meal_type_id    INTEGER GENERATED ALWAYS AS IDENTITY  PRIMARY KEY,
    name            TEXT NOT NULL UNIQUE,
    display_name    TEXT NOT NULL,
    sort_order      INTEGER NOT NULL DEFAULT 0
);
COMMENT ON TABLE meal_types IS 'Defines types of meals (e.g., breakfast, lunch, dinner, snack)';    
COMMENT ON COLUMN meal_types.name IS 'The internal name of the meal type, used for referencing in code';
COMMENT ON COLUMN meal_types.display_name IS 'The user-friendly name of the meal type, displayed in the UI';
COMMENT ON COLUMN meal_types.sort_order IS 'Determines the order in which meal types are displayed in the UI';
INSERT INTO meal_types (name, display_name, sort_order) VALUES
    ('breakfast', 'Breakfast', 1),
    ('lunch', 'Lunch', 2),
    ('dinner', 'Dinner', 3),
    ('snack', 'Snack', 4);

CREATE TABLE IF NOT EXISTS item_types (
    item_type_id    INTEGER GENERATED ALWAYS AS IDENTITY  PRIMARY KEY,
    name            TEXT NOT NULL UNIQUE,
    display_name    TEXT NOT NULL,
    sort_order      INTEGER NOT NULL DEFAULT 0
);
COMMENT ON TABLE item_types IS 'Defines types of items (e.g., recipe, homemade, store-bought)';    
COMMENT ON COLUMN item_types.name IS 'The internal name of the item type, used for referencing in code';
COMMENT ON COLUMN item_types.display_name IS 'The user-friendly name of the item type, displayed in the UI';
COMMENT ON COLUMN item_types.sort_order IS 'Determines the order in which item types are displayed in the UI';
INSERT INTO item_types (name, display_name, sort_order) VALUES
    ('recipe', 'Recipe', 1),
    ('homemade', 'Homemade', 2),
    ('store_bought', 'Store-bought', 3);

CREATE TABLE IF NOT EXISTS item_statuses (
    item_status_id      INTEGER GENERATED ALWAYS AS IDENTITY  PRIMARY KEY,
    name                TEXT NOT NULL UNIQUE,
    display_name        TEXT NOT NULL,
    sort_order          INTEGER NOT NULL DEFAULT 0
);
COMMENT ON TABLE item_statuses IS 'Defines statuses for meal items (e.g., confirmed, pending, unknown)';    
COMMENT ON COLUMN item_statuses.name IS 'The internal name of the item status, used for referencing in code';
COMMENT ON COLUMN item_statuses.display_name IS 'The user-friendly name of the item status, displayed in the UI';
COMMENT ON COLUMN item_statuses.sort_order IS 'Determines the order in which item statuses are displayed in the UI';
INSERT INTO item_statuses (name, display_name, sort_order) VALUES
    ('confirmed', 'Confirmed', 1),
    ('pending', 'Pending', 2),
    ('unknown', 'Unknown', 3);


CREATE TABLE IF NOT EXISTS resource_types (
    resource_type_id        INTEGER GENERATED ALWAYS AS IDENTITY  PRIMARY KEY,
    name                    TEXT NOT NULL UNIQUE,
    display_name            TEXT NOT NULL,
    sort_order              INTEGER NOT NULL DEFAULT 0
);
COMMENT ON TABLE resource_types IS 'Defines types of resources (e.g., recipe, meal, plan)';    
COMMENT ON COLUMN resource_types.name IS 'The internal name of the resource type, used for referencing in code';
COMMENT ON COLUMN resource_types.display_name IS 'The user-friendly name of the resource type, displayed in the UI';
COMMENT ON COLUMN resource_types.sort_order IS 'Determines the order in which resource types are displayed in the UI';
INSERT INTO resource_types (name, display_name, sort_order) VALUES
    ('recipe', 'Recipe', 1),
    ('meal', 'Meal', 2),
    ('plan', 'Plan', 3);
     

CREATE TABLE IF NOT EXISTS subject_types (
    subject_type_id     INTEGER GENERATED ALWAYS AS IDENTITY  PRIMARY KEY,
    name                TEXT NOT NULL UNIQUE,
    display_name        TEXT NOT NULL,
    sort_order          INTEGER NOT NULL DEFAULT 0
);
COMMENT ON TABLE subject_types IS 'Defines types of subjects (e.g., user, group)';    
COMMENT ON COLUMN subject_types.name IS 'The internal name of the subject type, used for referencing in code';
COMMENT ON COLUMN subject_types.display_name IS 'The user-friendly name of the subject type, displayed in the UI';
COMMENT ON COLUMN subject_types.sort_order IS 'Determines the order in which subject types are displayed in the UI';
INSERT INTO subject_types (name, display_name, sort_order) VALUES
    ('user', 'User', 1),
    ('group', 'Group', 2);

CREATE TABLE IF NOT EXISTS permission_types (
    permission_id           INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name                    TEXT NOT NULL UNIQUE,
    display_name            TEXT NOT NULL,
    sort_order              INT NOT NULL DEFAULT 0
);
INSERT INTO permission_types (name, display_name, sort_order) VALUES
    ('view', 'View', 1),
    ('edit', 'Edit', 2),
    ('comment', 'Comment', 3),
    ('manage', 'Manage', 4);

-- resource permissions
CREATE TABLE IF NOT EXISTS resource_permissions (

    resource_permission_id  UUID PRIMARY KEY,
    resource_type_id        INTEGER NOT NULL REFERENCES resource_types(resource_type_id),
    resource_id             INTEGER,
    subject_type_id         INTEGER NOT NULL REFERENCES subject_types(subject_type_id),
    subject_id              UUID,                 
    permission_type_id      INTEGER NOT NULL REFERENCES permission_types(permission_id),
    granted_by              UUID NOT NULL,
    created_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    expires_at              TIMESTAMP WITH TIME ZONE,
    updated_by              UUID NOT NULL,
    updated_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,

    UNIQUE (resource_type_id, resource_id, subject_type_id, subject_id),
    CHECK  (expires_at IS NULL OR expires_at > created_at)
);
COMMENT ON TABLE resource_permissions IS 'Defines permissions for resources, allowing for flexible access control based on resource type and subject type';
COMMENT ON COLUMN resource_permissions.resource_type_id IS 'The ID of the resource type (e.g., recipe, meal, plan) that the permission applies to, referencing resource_types';
COMMENT ON COLUMN resource_permissions.resource_id IS 'The specific ID of the resource that the permission applies to';
COMMENT ON COLUMN resource_permissions.subject_type_id IS 'The ID of the subject type (e.g., user, group) that the permission applies to, referencing subject_types';
COMMENT ON COLUMN resource_permissions.subject_id IS 'The specific ID of the subject that the permission applies to';
COMMENT ON COLUMN resource_permissions.permission_type_id IS 'The level of permission granted (e.g., read, write, admin), referencing permissions';
COMMENT ON COLUMN resource_permissions.granted_by IS 'The user ID of the person who granted the permission from the Identity DB';
COMMENT ON COLUMN resource_permissions.created_at IS 'The timestamp when the permission was granted';
COMMENT ON COLUMN resource_permissions.expires_at IS 'The timestamp when the permission expires, if applicable';
COMMENT ON COLUMN resource_permissions.updated_by IS 'The user ID of the person who last updated the permission details from the Identity DB';
COMMENT ON COLUMN resource_permissions.updated_at IS 'The timestamp when the permission was last updated';  
-- recipes
CREATE TABLE IF NOT EXISTS recipes(
    recipe_id           UUID PRIMARY KEY,
    description         TEXT,
    notes               TEXT,
    name                TEXT,
    ranking             INTEGER,
    original_source     TEXT,
    cook_time           TIME,
    prep_time           TIME,
    servings            INT,
    is_composite        BOOLEAN,
    owner_user_id       UUID ,
    created_at          TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at          TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by          UUID,
    updated_by          UUID,
    deleted_at          TIMESTAMP WITH TIME ZONE NULL
);
COMMENT ON TABLE recipes IS 'Stores recipe information, including metadata and ownership details';
COMMENT ON COLUMN recipes.is_composite IS 'Indicates whether the recipe is a composite of other recipes (true) or a simple recipe (false)';
COMMENT ON COLUMN recipes.owner_user_id IS 'The user ID of the owner/creator of the recipe, referencing the users table in the Identity DB';
COMMENT ON COLUMN recipes.deleted_at IS 'The timestamp when the recipe was deleted. A NULL value indicates that the recipe is active, while a non-NULL value indicates that it has been soft-deleted';
COMMENT ON COLUMN recipes.created_by IS 'The user ID of the person who created the recipe from the Identity DB';
COMMENT ON COLUMN recipes.updated_by IS 'The user ID of the person who last updated the recipe from the Identity DB';


--recipe components (definition layer — what makes up a recipe)
CREATE TABLE IF NOT EXISTS recipe_components (
    recipe_component_id     UUID PRIMARY KEY,
    parent_recipe_id        UUID NOT NULL REFERENCES recipes(recipe_id),
    child_recipe_id         UUID REFERENCES recipes(recipe_id),
    sorting_order           INTEGER,
    assembly_notes          TEXT,
    created_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by              UUID,
    updated_by              UUID
);
COMMENT ON TABLE recipe_components IS 'Defines the components of a composite recipe, allowing for hierarchical relationships between recipes';
COMMENT ON COLUMN recipe_components.parent_recipe_id IS 'The ID of the parent recipe that includes the component, referencing the recipes table';
COMMENT ON COLUMN recipe_components.child_recipe_id IS 'The ID of the child recipe that is a component of the parent recipe, referencing the recipes table. A NULL value indicates that the component is not a recipe but rather an ingredient or other item';
COMMENT ON COLUMN recipe_components.sorting_order IS 'Determines the order in which components are assembled or displayed within the parent recipe';
COMMENT ON COLUMN recipe_components.assembly_notes IS 'Additional notes or instructions for assembling the component within the parent recipe';
COMMENT ON COLUMN recipe_components.created_by IS 'The user ID of the person who created the recipe component';
COMMENT ON COLUMN recipe_components.updated_by IS 'The user ID of the person who last updated the recipe component';
-- recipe instructions
CREATE TABLE IF NOT EXISTS recipe_instructions (
    instruction_id          UUID PRIMARY KEY,
    recipe_id               UUID NOT NULL REFERENCES recipes(recipe_id),
    description             TEXT,
    step_number             INTEGER,
    note                    TEXT,
    created_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by              UUID,
    updated_by UUID
);
COMMENT ON TABLE recipe_instructions IS 'Stores step-by-step instructions for preparing a recipe';
COMMENT ON COLUMN recipe_instructions.recipe_id IS 'The ID of the recipe that the instruction belongs to, referencing the recipes table';
COMMENT ON COLUMN recipe_instructions.step_number IS 'The sequential number of the instruction step, determining the order in which instructions should be followed';
COMMENT ON COLUMN recipe_instructions.created_by IS 'The user ID of the person who created the instruction from the Identity DB';
COMMENT ON COLUMN recipe_instructions.updated_by IS 'The user ID of the person who last updated the instruction from the Identity DB';

-- recipe ingredients
CREATE TABLE IF NOT EXISTS recipe_ingredients (
    ingredient_id           UUID PRIMARY KEY,
    name                    TEXT,
    amount                  NUMERIC,
    measurement_type        TEXT,
    note                    TEXT,
    recipe_id               UUID NOT NULL REFERENCES recipes(recipe_id),
    created_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by              UUID,
    updated_by              UUID 
);
COMMENT ON TABLE recipe_ingredients IS 'Stores ingredients for recipes, including quantity and measurement details';
COMMENT ON COLUMN recipe_ingredients.recipe_id IS 'The ID of the recipe that the ingredient belongs to, referencing the recipes table';
COMMENT ON COLUMN recipe_ingredients.created_by IS 'The user ID of the person who created the ingredient from the Identity DB';
COMMENT ON COLUMN recipe_ingredients.updated_by IS 'The user ID of the person who last updated the ingredient from the Identity DB'; 
COMMENT ON COLUMN recipe_ingredients.amount IS 'The quantity of the ingredient required for the recipe';
COMMENT ON COLUMN recipe_ingredients.measurement_type IS 'The unit of measurement for the ingredient amount (e.g., grams, cups, tablespoons)';  
COMMENT ON COLUMN recipe_ingredients.name IS 'The name of the ingredient, which can be a specific item or a general description (e.g., "chopped onions", "flour")';
COMMENT ON COLUMN recipe_ingredients.note IS 'Additional notes or instructions related to the ingredient, such as preparation details (e.g., "chopped", "divided") or substitutions';
COMMENT ON COLUMN recipe_ingredients.ingredient_id IS 'The unique identifier for the ingredient, generated as a serial number';
COMMENT ON COLUMN recipe_ingredients.created_at IS 'The timestamp when the ingredient was created';
COMMENT ON COLUMN recipe_ingredients.updated_at IS 'The timestamp when the ingredient was last updated'; 


-- meals
CREATE TABLE IF NOT EXISTS meals (
    meal_id                 UUID PRIMARY KEY,
    name                    TEXT,
    description             TEXT,
    notes                   TEXT,
    meal_type_id            INT REFERENCES meal_types(meal_type_id),
    is_multi_day_meal       BOOLEAN,
    owner_user_id           UUID,
    created_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    deleted_at              TIMESTAMP WITH TIME ZONE NULL,
    created_by              UUID,
    updated_by              UUID
);
COMMENT ON TABLE meals IS 'Stores meal information, including metadata and ownership details';
COMMENT ON COLUMN meals.meal_type_id IS 'The type of meal (e.g., breakfast, lunch, dinner, snack), referencing meal_types';
COMMENT ON COLUMN meals.is_multi_day_meal IS 'Indicates whether the meal spans multiple days (true) or is contained within a single day (false)';
COMMENT ON COLUMN meals.owner_user_id IS 'The user ID of the owner/creator of the meal, referencing the users table in the Identity DB';
COMMENT ON COLUMN meals.deleted_at IS 'The timestamp when the meal was deleted. A NULL value indicates that the meal is active, while a non-NULL value indicates that it has been soft-deleted';
COMMENT ON COLUMN meals.created_by IS 'The user ID of the person who created the meal from the Identity DB';
COMMENT ON COLUMN meals.updated_by IS 'The user ID of the person who last updated the meal from the Identity DB';    
 -- meal items (definition layer — what makes up a meal)
CREATE TABLE IF NOT EXISTS meal_items (
    meal_item_id            UUID PRIMARY KEY,
    meal_id                 UUID NOT NULL REFERENCES meals(meal_id),
    name                    TEXT,
    recipe_id               UUID REFERENCES recipes(recipe_id),
    item_type_id            INT REFERENCES item_types(item_type_id),
    created_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by              UUID,
    updated_by              UUID
);
COMMENT ON TABLE meal_items IS 'Defines the items that make up a meal, which can be recipes or other types of items';
COMMENT ON COLUMN meal_items.meal_id IS 'The ID of the meal that the item belongs to, referencing the meals table';
COMMENT ON COLUMN meal_items.recipe_id IS 'The ID of the recipe associated with the meal item, referencing the recipes table. A NULL value indicates that the item is not a recipe but rather a homemade or store-bought item';
COMMENT ON COLUMN meal_items.item_type_id IS 'The type of item (e.g., recipe, homemade, store-bought), referencing item_types';
COMMENT ON COLUMN meal_items.created_by IS 'The user ID of the person who created the meal item';
COMMENT ON COLUMN meal_items.updated_by IS 'The user ID of the person who last updated the meal item';
CREATE TABLE IF NOT EXISTS cached_users (
    user_id             UUID PRIMARY KEY, -- The exact UUID from the Identity DB
    display_name        VARCHAR(100) NOT NULL,
    synced_at           TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE cached_users IS 'Caches user information from the Identity DB to optimize performance and reduce cross-database queries';
COMMENT ON COLUMN cached_users.user_id IS 'The unique identifier for the user, matching the UUID from the Identity DB';
COMMENT ON COLUMN cached_users.display_name IS 'The display name of the user, used for showing user information in the UI without needing to query the Identity DB';
COMMENT ON COLUMN cached_users.synced_at IS 'The timestamp when the user information was last synced with the Identity DB, allowing for tracking of data freshness and determining when to refresh the cache';

CREATE TABLE IF NOT EXISTS cached_groups (
    group_id            UUID PRIMARY KEY, -- The exact UUID from the Identity DB
    group_name          VARCHAR(200) NOT NULL,
    synced_at           TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE cached_groups IS 'Caches group information from the Identity DB to optimize performance and reduce cross-database queries';
COMMENT ON COLUMN cached_groups.group_id IS 'The unique identifier for the group, matching the UUID from the Identity DB';
COMMENT ON COLUMN cached_groups.group_name IS 'The name of the group, used for showing group information in the UI without needing to query the Identity DB';
COMMENT ON COLUMN cached_groups.synced_at IS 'The timestamp when the group information was last synced with the Identity DB, allowing for tracking of data freshness and determining when to refresh the cache';

CREATE Table IF NOT EXISTS cached_group_members(
    cached_group_member_id          UUID PRIMARY KEY,   -- The exact UUID from Identity DB
    user_id                         UUID,               -- The exact UUID from Identity DB
    group_id                        UUID,               -- The exact UUID from Identity DB
    role_id                         INTEGER,
    role_name                       TEXT,
    status_id                       INTEGER,
    status_name                     TEXT,
    synced_at                       TIMESTAMP NOT NULL DEFAULT CURRENT_TIME

);
COMMENT ON TABLE cached_group_members IS 'Caches group member information from the Identity DB to optimize performance and reduce cross-database queries';
COMMENT ON COLUMN cached_group_members.cached_group_member_id IS 'The unique identifier for the group member, matching the UUID from the Identity DB';
COMMENT ON COLUMN cached_group_members.user_id IS 'The unique identifier for the user, matching the UUID from the Identity DB';
COMMENT ON COLUMN cached_group_members.group_id IS 'The unique identifier for the group, matching the UUID from the Identity DB';
COMMENT ON COLUMN cached_group_members.role_id IS 'The  identifier for the member role, matching the integer value from a lookup table in the Identity DB';
COMMENT ON COLUMN cached_group_members.role_name IS 'The string value for the member role, matching the string value from a lookup table in the Identity DB';
COMMENT ON COLUMN cached_group_members.status_id IS 'The identifier for the member status, matching the integer value from a lookup table in the Identity DB';
COMMENT ON COLUMN cached_group_members.status_name IS 'The string value for the member status, matching the string value from a lookup table in the Identity DB';
COMMENT ON COLUMN cached_group_members.synced_at IS 'The last time the cached value was synced from the Identity DB';
