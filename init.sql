/*SOMTHING BUT ALSO NOTHING*/
-- Create the login user
CREATE USER identity_app
    WITH PASSWORD 'REPLACE_WITH_STRONG_PASSWORD'
    NOSUPERUSER
    NOCREATEDB
    NOCREATEROLE
    NOINHERIT
    LOGIN;


\c your_database_name;

-- Schema access
GRANT CONNECT ON DATABASE your_database_name TO identity_app;
GRANT USAGE ON SCHEMA public TO identity_app;

-- /register: FindByNameAsync + CreateAsync
GRANT SELECT, INSERT ON "AspNetUsers" TO identity_app;

-- /login: CheckPasswordSignInAsync reads + writes lockout fields
GRANT UPDATE ("AccessFailedCount", "LockoutEnd", "LockoutEnabled")
    ON "AspNetUsers" TO identity_app;

-- CreateAsync writes a security stamp claim on register
GRANT SELECT, INSERT ON "AspNetUserClaims" TO identity_app;

-- EF / Identity uses sequences for any generated IDs
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO identity_app;

-- Explicitly deny everything else — belt and braces
REVOKE ALL ON "AspNetRoles"      FROM identity_app;
REVOKE ALL ON "AspNetUserRoles"  FROM identity_app;
REVOKE ALL ON "AspNetRoleClaims" FROM identity_app;
REVOKE ALL ON "AspNetUserLogins" FROM identity_app;
REVOKE ALL ON "AspNetUserTokens" FROM identity_app;