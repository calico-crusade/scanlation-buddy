CREATE TABLE IF NOT EXISTS buddy_user_role (
	id INTEGER PRIMARY KEY,

	user_id INTEGER NOT NULL,
	role_id INTEGER NOT NULL,
	creator_id INTEGER NOT NULL,

	created_at TEXT NOT NULL,
	updated_at TEXT NOT NULL,
	deleted_at TEXT,

	FOREIGN KEY(creator_id) REFERENCES buddy_user(id),
	FOREIGN KEY(user_id) REFERENCES buddy_user(id),
	FOREIGN KEY(role_id) REFERENCES buddy_role(id),
	UNIQUE(user_id, role_id)
)