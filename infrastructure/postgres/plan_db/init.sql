-- PostgreSQL initialization script for Plan Service DB

-- extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- lookup tables (replace enum types)

 

CREATE TABLE IF NOT EXISTS meal_item_plan_status_types (
    meal_item_plan_status_id        INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name                            TEXT NOT NULL UNIQUE,
    display_name                    TEXT NOT NULL,
    sort_order                      INT NOT NULL DEFAULT 0
);
INSERT INTO meal_item_plan_status_types (name, display_name, sort_order) VALUES
    ('confirmed', 'Confirmed', 1),
    ('pending', 'Pending', 2),
    ('unknown', 'Unknown', 3);

COMMENT ON TABLE meal_item_plan_status_types IS 'Defines the possible status values for meal items within a plan, allowing for tracking of whether a meal item is confirmed, pending, or unknown in terms of who is responsible for it and whether it will be brought or made for a scheduled meal. This helps users manage their meal plans more effectively by providing clear information about the status of each meal item assignment.';
COMMENT ON COLUMN meal_item_plan_status_types.name IS 'The unique name of the meal item plan status type, used for referencing the status in meal item assignments and plan management logic.';
COMMENT ON COLUMN meal_item_plan_status_types.display_name IS 'The human-readable name of the meal item plan status type, used for displaying the status in the user interface and for better understanding of the meal item assignment statuses when managing meal plans.';
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


CREATE TABLE IF NOT EXISTS resource_types (
    resource_type_id        INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name                    TEXT NOT NULL UNIQUE,
    display_name            TEXT NOT NULL,
    sort_order              INT NOT NULL DEFAULT 0
);
COMMENT ON TABLE resource_types IS 'Defines the types of resources that permissions can be assigned to, such as recipes, meals, and plans. This allows for flexible access control by categorizing resources and associating permissions with them based on their type.';
COMMENT ON COLUMN resource_types.name IS 'The unique name of the resource type, used for referencing the type in permission assignments and access control logic.';
COMMENT ON COLUMN resource_types.display_name IS 'The human-readable name of the resource type, used for displaying the type in the user interface and for better understanding of the resource categories when managing permissions.'; 

INSERT INTO resource_types (name, display_name, sort_order) VALUES
    ('recipe', 'Recipe', 1),
    ('meal', 'Meal', 2),
    ('plan', 'Plan', 3);

CREATE TABLE IF NOT EXISTS subject_types (
    subject_type_id     INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name                TEXT NOT NULL UNIQUE,
    display_name        TEXT NOT NULL,
    sort_order          INT NOT NULL DEFAULT 0
);
COMMENT ON TABLE subject_types IS 'Defines the types of subjects that can be granted permissions on resources, such as users or groups. This allows for flexible access control by categorizing subjects and associating permissions with them based on their type.';   
COMMENT ON COLUMN subject_types.name IS 'The unique name of the subject type, used for referencing the type in permission assignments and access control logic.';
COMMENT ON COLUMN subject_types.display_name IS 'The human-readable name of the subject type, used for displaying the type in the user interface and for better understanding of the subject categories when managing permissions.';    
INSERT INTO subject_types (name, display_name, sort_order) VALUES
    ('user', 'User', 1),
    ('group', 'Group', 2);
 


-- resource permissions
CREATE TABLE IF NOT EXISTS resource_permissions (

    resource_permission_id  UUID PRIMARY KEY,
    resource_type           TEXT NOT NULL REFERENCES resource_types(name),
    resource_id             int,
    subject_type_id         INTEGER NOT NULL REFERENCES subject_types(subject_type_id),
    subject_id              UUID,                 
    permission_type_id      INTEGER NOT NULL REFERENCES permission_types(permission_id),
    granted_by              UUID NOT NULL,
    created_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    expires_at              TIMESTAMP WITH TIME ZONE,
    updated_by              UUID NOT NULL,
    updated_at              TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,

    UNIQUE (resource_type, resource_id, subject_type_id, subject_id),
    CHECK  (expires_at IS NULL OR expires_at > created_at)
);
COMMENT ON TABLE resource_permissions IS 'Defines permissions for resources, allowing for flexible access control based on resource type and subject type';
COMMENT ON COLUMN resource_permissions.resource_type IS 'The type of resource (e.g., recipe, meal, plan) that the permission applies to, referencing resource_types';
COMMENT ON COLUMN resource_permissions.resource_id IS 'The specific ID of the resource that the permission applies to';
COMMENT ON COLUMN resource_permissions.subject_type_id IS 'The type of subject (e.g., user, group) that the permission applies to, referencing subject_types';
COMMENT ON COLUMN resource_permissions.subject_id IS 'The specific ID of the subject that the permission applies to';
COMMENT ON COLUMN resource_permissions.permission_type_id IS 'The level of permission granted (e.g., read, write, admin), referencing permission_types';
COMMENT ON COLUMN resource_permissions.granted_by IS 'The user ID of the person who granted the permission from the Identity DB';
COMMENT ON COLUMN resource_permissions.created_at IS 'The timestamp when the permission was granted';
COMMENT ON COLUMN resource_permissions.expires_at IS 'The timestamp when the permission expires, if applicable';
COMMENT ON COLUMN resource_permissions.updated_by IS 'The user ID of the person who last updated the permission details from the Identity DB';
COMMENT ON COLUMN resource_permissions.updated_at IS 'The timestamp when the permission was last updated';  
-- plans
CREATE TABLE IF NOT EXISTS plans (
    plan_id             UUID PRIMARY KEY,
    start_date          DATE,
    end_date            DATE,
    name                TEXT,
    owner_user_id       UUID,
    created_at          TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at          TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by          UUID,
    updated_by          UUID
);
COMMENT ON TABLE plans IS 'Represents a meal plan, which can include multiple scheduled meals and associated details';
COMMENT ON COLUMN plans.start_date IS 'The start date of the meal plan, indicating when the plan begins and meals can be scheduled from this date onward';
COMMENT ON COLUMN plans.end_date IS 'The end date of the meal plan, indicating when the plan ends and meals can no longer be scheduled after this date';
COMMENT ON COLUMN plans.name IS 'The name of the meal plan, used for identification and display purposes';
COMMENT ON COLUMN plans.owner_user_id IS 'The user ID of the owner of the meal plan, referencing the Identity DB to establish ownership and permissions for managing the plan';
COMMENT ON COLUMN plans.created_at IS 'The timestamp when the meal plan was created';
COMMENT ON COLUMN plans.updated_at IS 'The timestamp when the meal plan was last updated';
COMMENT ON COLUMN plans.created_by IS 'The user ID of the person who created the meal plan from the Identity DB';
COMMENT ON COLUMN plans.updated_by IS 'The user ID of the person who last updated the meal plan from the Identity DB';

-- meal plan scheduling
CREATE TABLE IF NOT EXISTS plan_meals (
    plan_meal_id        UUID PRIMARY KEY,
    meal_id             UUID,
    plan_id             UUID NOT NULL REFERENCES plans(plan_id),
    serve_date          DATE,
    end_date            DATE,
    added_by_user_id    UUID,
    created_at          TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at          TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by          UUID,
    updated_by          UUID,
    UNIQUE (meal_id, plan_id, serve_date)
);
COMMENT ON TABLE plan_meals IS 'Represents the scheduling of a meal within a plan, allowing for the same meal to be scheduled multiple times on different dates or within different plans, but enforcing that the same meal cannot be scheduled more than once on the same date within the same plan';  
COMMENT ON COLUMN plan_meals.meal_id IS 'The ID of the meal being scheduled, referencing the meal service';
COMMENT ON COLUMN plan_meals.plan_id IS 'The ID of the plan that this meal is part of, referencing the plans table';
COMMENT ON COLUMN plan_meals.serve_date IS 'The date on which the meal is scheduled to be served, allowing for scheduling of meals on specific dates within a plan';
COMMENT ON COLUMN plan_meals.end_date IS 'The optional end date for the meal schedule, allowing for meals to be scheduled for a range of dates (e.g., a meal that is served for multiple days)';
COMMENT ON COLUMN plan_meals.added_by_user_id IS 'The user ID of the person who added the meal to the plan from the Identity DB';
COMMENT ON COLUMN plan_meals.created_at IS 'The timestamp when the meal was added to the plan';
COMMENT ON COLUMN plan_meals.updated_at IS 'The timestamp when the meal schedule was last updated';
COMMENT ON  COLUMN plan_meals.created_by IS 'The user ID of the person who created the meal schedule from the Identity DB';
COMMENT ON  COLUMN plan_meals.updated_by IS 'The user ID of the person who last updated the meal schedule from the Identity DB';


-- meal plan items (assignment layer — who brings/makes what for a specific scheduled instance)
CREATE TABLE IF NOT EXISTS plan_meal_items (
    plan_meal_item_id           UUID PRIMARY KEY,
    plan_meal_id                UUID NOT NULL REFERENCES plan_meals(plan_meal_id),
    meal_item_id                UUID NOT NULL,
    assigned_to_user            UUID,
    assigned_to_guest_name      TEXT,
    status_id                   INT REFERENCES meal_item_plan_status_types(meal_item_plan_status_id),
    notes                       TEXT,
    created_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by                  UUID,
    updated_by                  UUID
);
COMMENT ON TABLE plan_meal_items IS 'Represents the assignment of a specific meal item to a user or guest for a scheduled meal within a plan, allowing for tracking of who is responsible for bringing or making each item for a specific meal instance';
COMMENT ON COLUMN plan_meal_items.plan_meal_id IS 'The ID of the scheduled meal that this meal item is part of, referencing the plan_meals table';
COMMENT ON COLUMN plan_meal_items.meal_item_id IS 'The ID of the meal item being assigned, referencing the meal service';
COMMENT ON COLUMN plan_meal_items.assigned_to_user IS 'The user ID of the person assigned to bring or make this meal item, referencing the Identity DB';
COMMENT ON COLUMN plan_meal_items.assigned_to_guest_name IS 'The name of the guest assigned to bring or make this meal item, used when the assigned person is not a registered user in the system';
COMMENT ON COLUMN plan_meal_items.status_id IS 'The status of the meal item assignment, referencing the meal_item_plan_status_types table to indicate whether the item is confirmed, pending, or unknown';
COMMENT ON COLUMN plan_meal_items.notes IS 'Any additional notes or details about the meal item assignment, such as dietary restrictions, preparation instructions, or other relevant information';
COMMENT ON COLUMN plan_meal_items.created_at IS 'The timestamp when the meal item was assigned to the scheduled meal';
COMMENT ON COLUMN plan_meal_items.updated_at IS 'The timestamp when the meal item assignment was last updated';
COMMENT ON COLUMN plan_meal_items.created_by IS 'The user ID of the person who created the meal item assignment from the Identity DB';
COMMENT ON COLUMN plan_meal_items.updated_by IS 'The user ID of the person who last updated the meal item assignment from the Identity DB';


-- cached identity data for performance optimization (denormalization)
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
    group_id             UUID PRIMARY KEY, -- The exact UUID from the Identity DB
    group_name           VARCHAR(200) NOT NULL,
    synced_at            TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
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



