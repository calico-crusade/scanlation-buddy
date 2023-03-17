CREATE TABLE IF NOT EXISTS buddy_config (
	id INTEGER PRIMARY KEY,

	key TEXT NOT NULL,
	value TEXT NOT NULL,
	description TEXT NOT NULL,
	group_name TEXT NOT NULL,

	created_at TEXT NOT NULL,
	updated_at TEXT NOT NULL,
	deleted_at TEXT,

	UNIQUE(key)
);

