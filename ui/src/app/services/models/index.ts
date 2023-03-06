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