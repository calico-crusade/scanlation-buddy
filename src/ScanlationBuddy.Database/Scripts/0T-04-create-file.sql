CREATE TABLE IF NOT EXISTS buddy_file (
	id INTEGER PRIMARY KEY,

	filename TEXT NOT NULL,
	hash TEXT NOT NULL,
	mime_type TEXT NOT NULL,
	length INTEGER NOT NULL,
	creator_id INTEGER NOT NULL,

	created_at TEXT NOT NULL,
	updated_at TEXT NOT NULL,
	deleted_at TEXT,

	FOREIGN KEY(creator_id) REFERENCES buddy_user(id)
)