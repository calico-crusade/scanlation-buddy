CREATE TABLE IF NOT EXISTS buddy_user (
	id INTEGER PRIMARY KEY,

	username TEXT NOT NULL,
    avatar TEXT NOT NULL,
    platform_id TEXT NOT NULL,
    email TEXT NOT NULL,
    provider TEXT NOT NULL,
    provider_id TEXT NOT NULL,

	created_at TEXT NOT NULL,
	updated_at TEXT NOT NULL,
	deleted_at TEXT
)