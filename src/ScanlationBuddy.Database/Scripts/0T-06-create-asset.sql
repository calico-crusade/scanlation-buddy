CREATE TABLE IF NOT EXISTS buddy_asset (
	id INTEGER PRIMARY KEY,

	file_id INTEGER
	replaced_by INTEGER REFERENCES buddy_asset(id),
	replaced_with INTEGER REFERENCES buddy_asset(id),
	name TEXT NOT NULL,
	description TEXT NOT NULL,
	creator_id INTEGER NOT NULL,

	created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	deleted_at TEXT,

	FOREIGN KEY(creator_id) REFERENCES buddy_user(id),
	FOREIGN KEY(file_id) REFERENCES buddy_file(id),

	UNIQUE (name, deleted_at)
)