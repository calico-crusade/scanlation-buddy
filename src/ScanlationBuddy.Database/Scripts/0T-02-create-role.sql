CREATE TABLE IF NOT EXISTS buddy_role (
	id INTEGER PRIMARY KEY,

	name TEXT NOT NULL,
    permissions TEXT NOT NULL,
    creator_id INTEGER,
    description TEXT NOT NULL,
	badge_id TEXT,
	color TEXT NOT NULL,

	created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	deleted_at TEXT,

	FOREIGN KEY(creator_id) REFERENCES buddy_user(id),
	UNIQUE(name, deleted_at)
)