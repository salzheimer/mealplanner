-- PostgreSQL initialization script for Identity Service DB

-- extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- lookup tables (replace enum types)

CREATE TABLE IF NOT EXISTS group_member_role_types (
    group_member_role_id INTEGER GENERATED ALWAYS AS IDENTITY  PRIMARY KEY,
    name TEXT NOT NULL UNIQUE,
    display_name TEXT NOT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0
);

Comment on TABLE group_member_role_types is 'reference table for defining the roles that users can have within a group. The ''owner'' role should have permissions to manage the group, including adding/removing members and changing member roles. The ''member'' role should have permissions to participate in the group but not manage it.';
Comment on COLUMN group_member_role_types.name is 'the name (text lookup) of the role, such as ''owner'' or ''member''.';
Comment on COLUMN group_member_role_types.display_name is 'the display name of the role, such as ''Owner'' or ''Member''.';
Comment on COLUMN group_member_role_types.sort_order is 'the sort order of the role, which can be used to determine the hierarchy of roles. For example, the ''owner'' role may have a sort order of 1, while the ''member'' role may have a sort order of 2 to indicate that it is a lower role than the owner.';    
INSERT INTO group_member_role_types (name, display_name, sort_order) VALUES
    ('owner', 'Owner', 1),
    ('member', 'Member', 2);

CREATE TABLE IF NOT EXISTS group_member_status_types (
    group_member_status_id      INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name                        TEXT NOT NULL UNIQUE,
    display_name                TEXT NOT NULL,
    sort_order                  INTEGER NOT NULL DEFAULT 0
);
Comment on TABLE group_member_status_types is 'reference table for defining the statuses that users can have within a group. The ''pending'' status indicates that a user has been invited to join the group but has not yet accepted the invitation. The ''active'' status indicates that a user is an active member of the group. The ''removed'' status indicates that a user has been removed from the group and no longer has access to it.';
Comment on COLUMN group_member_status_types.name is 'the name (text lookup) of the status, such as ''pending'', ''active'', or ''removed''.';
Comment on COLUMN group_member_status_types.display_name is 'the display name of the status, such as ''Pending'', ''Active'', or ''Removed''.';
Comment on COLUMN group_member_status_types.sort_order is 'the sort order of the status, which can be used to determine the progression of statuses. For example, the ''pending'' status may have a sort order of 1, the ''active'' status may have a sort order of 2, and the ''removed'' status may have a sort order of 3 to indicate that it is the final status in the progression.';
INSERT INTO group_member_status_types (name, display_name, sort_order) VALUES
    ('pending', 'Pending', 1),
    ('active', 'Active', 2),
    ('removed', 'Removed', 3);
 
CREATE TABLE IF NOT EXISTS client_types (
    client_type_id          INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name                    TEXT NOT NULL UNIQUE,
    display_name            TEXT NOT NULL,
    sort_order              INTEGER NOT NULL DEFAULT 0
);
Comment on TABLE client_types is 'reference table for defining the types of clients that can access the application. The ''web'' client type represents users accessing the application through a web browser, the ''mobile'' client type represents users accessing the application through a mobile app, and the ''api'' client type represents users accessing the application through an API client.';
Comment on COLUMN client_types.name is 'the name (text lookup) of the client type, such as ''web'', ''mobile'', or ''api''.';
Comment on COLUMN client_types.display_name is 'the display name of the client type, such as ''Web'', ''Mobile'', or ''API''.';
Comment on COLUMN client_types.sort_order is 'the sort order of the client type, which can be used to determine the priority of client types. For example, the ''web'' client type may have a sort order of 1, the ''mobile'' client type may have a sort order of 2, and the ''api'' client type may have a sort order of 3 to indicate that it is the lowest priority client type.';
INSERT INTO client_types (name, display_name, sort_order) VALUES
    ('web', 'Web', 1),
    ('mobile', 'Mobile', 2),
    ('api', 'API', 3);


-- user
CREATE TABLE IF NOT EXISTS users (
    user_id                     UUID PRIMARY KEY, 
    display_name                TEXT,
    email                       TEXT NOT NULL UNIQUE,
    email_verified              BOOLEAN NOT NULL DEFAULT FALSE,
    email_verified_at           TIMESTAMP WITH TIME ZONE,
    created_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    last_login_at               TIMESTAMP WITH TIME ZONE,
    auth0_id                    TEXT,
    is_active                   BOOLEAN  DEFAULT TRUE,
    failed_login_attempts       INTEGER DEFAULT 0,
    locked_until                TIMESTAMP WITH TIME ZONE,
    terms_accepted_at           TIMESTAMP WITH TIME ZONE,
    terms_version               TEXT,
    security_stamp              TEXT
);
Comment on TABLE users is 'the main user table that stores basic information about each user, including their display name, email, and authentication details. ';    
Comment on COLUMN users.display_name is 'the display name of the user, which can be used for personalization and display purposes within the application.';
Comment on COLUMN users.email is 'the email address of the user, which is used for authentication and communication purposes. This field is unique to ensure that each user has a distinct email address.';
Comment on COLUMN users.email_verified is 'a boolean flag indicating whether the user''s email address has been verified. This is important for ensuring that the user has access to the email address they provided and can receive communications.';
Comment on COLUMN users.email_verified_at is 'the timestamp of when the user''s email address was verified.';
Comment on COLUMN users.created_at is 'the timestamp of when the user account was created.';
Comment on COLUMN users.updated_at is 'the timestamp of when the user account was last updated.';
Comment on COLUMN users.last_login_at is 'the timestamp of the user''s last login.';
Comment on COLUMN users.auth0_id is 'the ID of the user in the Auth0 authentication system, if applicable.';
Comment on COLUMN users.is_active is 'a boolean flag indicating whether the user account is active or has been deactivated.';
Comment on COLUMN users.failed_login_attempts is 'the number of consecutive failed login attempts for the user account.';
Comment on COLUMN users.locked_until is 'the timestamp until which the user account is locked due to too many failed login attempts.';
Comment on COLUMN users.terms_accepted_at is 'the timestamp of when the user accepted the terms of service.';
Comment on COLUMN users.terms_version is 'the version of the terms of service that the user accepted.';
Comment on COLUMN users.security_stamp is 'a value used to invalidate sessions when critical changes are made to the user''s account, such as a password change.';

-- groups
CREATE TABLE IF NOT EXISTS groups (
    group_id                    UUID PRIMARY KEY,
    name                        VARCHAR(200) NOT NULL,
    created_by_user_id          UUID REFERENCES users(user_id),
    created_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE groups IS 'the groups table that stores information about each group, including the group name and the user who created the group. This table is used to manage group memberships and permissions within the application.';
COMMENT ON COLUMN groups.name IS 'the name of the group, which is used to identify the group within the application. The group name should be unique to avoid confusion between groups.';
COMMENT ON COLUMN groups.created_by_user_id IS 'the ID of the user who created the group. This can be used to track group ownership and manage permissions for group management actions.';
COMMENT ON COLUMN groups.created_at IS 'the timestamp of when the group was created. This can be used for auditing and tracking purposes, as well as for sorting and displaying groups based on their creation date.'; 

-- group members
CREATE TABLE IF NOT EXISTS group_members (
    group_member_id             UUID PRIMARY KEY,
    user_id                     UUID NOT NULL REFERENCES users(user_id),
    group_id                    UUID NOT NULL REFERENCES groups(group_id),
    role_id                     INTEGER NOT NULL DEFAULT 2 REFERENCES group_member_role_types(group_member_role_id),
    invited_by_user_id          UUID REFERENCES users(user_id),
    invited_at                  TIMESTAMP WITH TIME ZONE,
    joined_at                   TIMESTAMP WITH TIME ZONE,
    removed_at                  TIMESTAMP WITH TIME ZONE,
    created_at                  TIMESTAMP WITH TIME ZONE,
    updated_at                  TIMESTAMP WITH TIME ZONE,
    status_id                   INTEGER NOT NULL DEFAULT 1 REFERENCES group_member_status_types(group_member_status_id)
);
COMMENT ON TABLE group_members IS 'the group_members table that manages the membership of users in groups, including their roles and statuses within the group. This table allows for flexible management of group memberships and permissions.';
COMMENT ON COLUMN group_members.user_id IS 'the ID of the user who is a member of the group. This field references the users table to establish a relationship between users and groups.';
COMMENT ON COLUMN group_members.group_id IS 'the ID of the group that the user is a member of. This field references the groups table to establish a relationship between users and groups.';
COMMENT ON COLUMN group_members.role_id IS 'the ID of the role that the user has within the group. This field references the group_member_role_types table to define the permissions and capabilities of the user within the group. The default role is set to 2, which corresponds to the "member" role.';
COMMENT ON COLUMN group_members.invited_by_user_id IS 'the ID of the user who invited the member to join the group. This field references the users table and can be used for auditing and tracking purposes to identify who is responsible for inviting members to groups.';
COMMENT ON COLUMN group_members.invited_at IS 'the timestamp of when the user was invited to join the group. This can be used for auditing and tracking purposes, as well as for managing invitations and determining how long a user has been invited to a group.';    
-- user credentials
CREATE TABLE IF NOT EXISTS user_credentials (
    user_credential_id          UUID PRIMARY KEY,
    user_id                     UUID NOT NULL REFERENCES users(user_id),
    password_hash               TEXT NOT NULL,
    hash_algorithm              TEXT NOT NULL,
    created_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE user_credentials IS 'the user_credentials table that stores the authentication credentials for each user, including the password hash and the hashing algorithm used. This table is used to manage user authentication and ensure the security of user accounts.';
COMMENT ON COLUMN user_credentials.user_id IS 'the ID of the user to whom the credentials belong. This field references the users table to establish a relationship between user credentials and user accounts.';
COMMENT ON COLUMN user_credentials.password_hash IS 'the hashed password of the user. This should be stored securely using a strong hashing algorithm to protect against unauthorized access to user accounts.';
COMMENT ON COLUMN user_credentials.hash_algorithm IS 'the name of the hashing algorithm used to hash the user''s password. This can be used to support multiple hashing algorithms and to identify the algorithm used for a particular user''s password hash.';
COMMENT ON COLUMN user_credentials.created_at IS 'the timestamp of when the user credentials were created. This can be used for auditing and tracking purposes, as well as for managing password expiration policies.';
COMMENT ON COLUMN user_credentials.updated_at IS 'the timestamp of when the user credentials were last updated. This can be used for auditing and tracking purposes, as well as for managing password expiration policies and ensuring that users update their passwords regularly for security reasons.';  
-- password resets
CREATE TABLE IF NOT EXISTS password_reset (
    password_reset_id           UUID PRIMARY KEY,
    user_id                     UUID NOT NULL REFERENCES users(user_id),
    token_hash                  TEXT NOT NULL,
    expires_at                  TIMESTAMP WITH TIME ZONE NOT NULL,
    used_at                     TIMESTAMP WITH TIME ZONE,
    created_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE password_reset IS 'the password_reset table that manages password reset requests for users, including the token hash, expiration time, and usage status. This table is used to facilitate secure password reset processes for users who have forgotten their passwords.';
COMMENT ON COLUMN password_reset.user_id is 'the ID of the user who requested the password reset. This field references the users table to establish a relationship between password reset requests and user accounts.';
COMMENT ON COLUMN password_reset.token_hash is 'the hashed token used for verifying the password reset request. This should be stored securely to prevent unauthorized access to password reset functionality.';
COMMENT ON COLUMN password_reset.expires_at is 'the timestamp of when the password reset token expires. This can be used to enforce a limited time window for password reset requests to enhance security and prevent misuse of expired tokens.';
COMMENT ON COLUMN password_reset.used_at is 'the timestamp of when the password reset token was used. This can be used to track the usage of password reset tokens and to prevent multiple uses of the same token, which can enhance security by ensuring that each token can only be used once.';
COMMENT ON COLUMN password_reset.created_at is 'the timestamp of when the password reset request was created. This can be used for auditing and tracking purposes, as well as for managing password reset request policies and ensuring that users receive timely responses to their password reset requests.';
-- sessions
CREATE TABLE IF NOT EXISTS sessions (
    session_id                  UUID PRIMARY KEY,
    user_id                     UUID NOT NULL REFERENCES users(user_id),
    token_hash                  TEXT NOT NULL,
    client_type_id              INTEGER NOT NULL REFERENCES client_types(client_type_id),
    device_info                 TEXT,
    ip_address                  TEXT,
    created_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at                  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    last_used_at                TIMESTAMP WITH TIME ZONE,
    revoked_at                  TIMESTAMP WITH TIME ZONE,
    expires_at                  TIMESTAMP WITH TIME ZONE NOT NULL
);
COMMENT ON TABLE sessions IS 'the session table that manages user sessions, including the session token, client type, device information, and session status. This table is used to manage user authentication sessions and to enhance security by tracking session activity and enforcing session expiration policies.';
COMMENT ON COLUMN sessions.user_id is 'the ID of the user associated with the session. This field references the users table to establish a relationship between user sessions and user accounts.';
COMMENT ON COLUMN sessions.token_hash is 'the hashed token used for verifying the session. This should be stored securely to prevent unauthorized access to session information and to enhance security by ensuring that session tokens are not stored in plain text.';
COMMENT ON COLUMN sessions.client_type_id is 'the ID of the client type associated with the session. This field references the client_types table to define the type of client (e.g., web, mobile, API) that is associated with the session. This can be used for auditing and tracking purposes, as well as for managing session policies based on client types.';
COMMENT ON COLUMN sessions.device_info is 'optional information about the device used for the session, such as the device type, operating system, or browser. This can be used for auditing and tracking purposes, as well as for enhancing security by identifying unusual device activity.';
COMMENT ON COLUMN sessions.ip_address is 'the IP address from which the session was initiated. This can be used for auditing and tracking purposes, as well as for enhancing security by identifying unusual IP address activity.';
COMMENT ON COLUMN sessions.created_at is 'the timestamp of when the session was created. This can be used for auditing and tracking purposes, as well as for managing session expiration policies.';
COMMENT ON COLUMN sessions.updated_at is 'the timestamp of when the session was last updated. This can be used for auditing and tracking purposes, as well as for managing session expiration policies and ensuring that sessions are kept up to date with user activity.';
COMMENT ON COLUMN sessions.last_used_at is 'the timestamp of when the session was last used. This can be used for auditing and tracking purposes, as well as for managing session expiration policies and identifying inactive sessions that may need to be revoked for security reasons.';
COMMENT ON COLUMN sessions.revoked_at is 'the timestamp of when the session was revoked. This can be used for auditing and tracking purposes, as well as for managing session revocation policies and ensuring   that revoked sessions are properly tracked and prevented from being used for authentication.';
COMMENT ON COLUMN sessions.expires_at is 'the timestamp of when the session expires. This can be used for auditing and tracking purposes, as well as for managing session expiration policies and ensuring that sessions are properly expired to enhance security by preventing long-lived sessions that may be vulnerable to unauthorized access.';
-- audit log
CREATE TABLE IF NOT EXISTS audit_logs (
    audit_log_id        UUID PRIMARY KEY,
    user_id             UUID REFERENCES users(user_id),
    session_id          UUID REFERENCES sessions(session_id),
    client_type_id      INT REFERENCES client_types(client_type_id),
    action              TEXT NOT NULL,
    ip_address          TEXT,
    resource_type       TEXT,
    resource_id         TEXT,
    metadata            JSONB,
    created_at          TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE audit_logs IS 'the audit_logs table that records important actions and events within the application, including user actions, session activity, and changes to resources. This table is used for auditing and tracking purposes to enhance security and provide insights into user behavior and system activity.';
COMMENT ON COLUMN audit_logs.user_id is 'the ID of the user associated with the audit log entry. This field references the users table to establish a relationship between audit log entries and user accounts.';
COMMENT ON COLUMN audit_logs.session_id is 'the ID of the session associated with the audit log entry. This field references the sessions table to establish a relationship between audit log entries and user sessions.';
COMMENT ON COLUMN audit_logs.client_type_id is 'the ID of the client type associated with the audit log entry. This field references the client_types table to define the type of client (e.g., web, mobile, API) that is associated with the audit log entry. This can be used for auditing and tracking purposes, as well as for managing session policies based on client types.';
COMMENT ON COLUMN audit_logs.action is 'the action performed within the application. This can be used for auditing and tracking purposes to understand user behavior and system activity.';
COMMENT ON COLUMN audit_logs.ip_address is 'the IP address from which the action was initiated. This can be used for auditing and tracking purposes, as well as for enhancing security by identifying unusual IP address activity.';
COMMENT ON COLUMN audit_logs.resource_type is 'the type of resource affected by the action. This can be used for auditing and tracking purposes to understand the scope of the action.';
COMMENT ON COLUMN audit_logs.resource_id is 'the ID of the resource affected by the action. This can be used for auditing and tracking purposes to understand the specific resource that was affected.';
COMMENT ON COLUMN audit_logs.metadata is 'additional metadata associated with the audit log entry. This can be used for auditing and tracking purposes to provide additional context about the action.';
COMMENT ON COLUMN audit_logs.created_at is 'the timestamp of when the audit log entry was created. This can be used for auditing and tracking purposes to understand when the action occurred.';