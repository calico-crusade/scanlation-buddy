export interface Paginated<T> {
    pages: number;
    count: number;
    results: T[];
}

export interface Buddy {
    id: number;
    createdAt?: Date;
    updatedAt?: Date;
    deletedAt?: Date;
}

export interface BuddyUser extends Buddy {
    username: string;
    avatar: string;
    platformId: string;
    provider: string;
    providerId: string;
}

export interface BuddyRole extends Buddy {
    name: string;
    description: string;
    permissions: string[];
    creatorId: number;
    badgeId?: string;
    color: string;
}

export interface BuddyPermission {
    name: string;
    description: string;
}

export interface BuddyUserRoles {
    user: BuddyUser;
    roles: BuddyRole[];
}

export interface BuddyConfig extends Buddy {
    key: string;
    value: string;
    description: string;
    groupName: string;
}

export interface BuddyFile extends Buddy {
    filename: string;
    hash: string;
    mimeType: string;
    length: number;
    creatorId: number;
}