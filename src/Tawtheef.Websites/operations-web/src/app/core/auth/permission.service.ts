import { Injectable, inject } from '@angular/core';
import { TokenService } from './token.service';

@Injectable({ providedIn: 'root' })
export class PermissionService {
    private readonly tokenService = inject(TokenService);
    private permissionsCache: { token: string; perms: Set<string> } | null = null;

    /**
     * Check if user has at least one / all of the required permissions.
     *
     * @param permission single permission or array
     * @param requireAll if true, all permissions must be present (AND); default: false (OR)
     */
    hasPermission(permission: string | string[], requireAll: boolean = false): boolean {
        const perms = this.getPermissionsFromToken();
        if (!permission) return false;

        const required = Array.isArray(permission) ? permission : [permission];

        if (required.length === 0) return false;
        if (perms.size === 0) return false;

        if (requireAll) {
            return required.every(p => perms.has(p));
        }

        return required.some(p => perms.has(p));
    }

    private getPermissionsFromToken(): Set<string> {
        const token = this.tokenService.getToken();
        if (!token) {
            this.permissionsCache = null;
            return new Set<string>();
        }

        // Simple cache validation
        if (this.permissionsCache && this.permissionsCache.token === token) {
            return this.permissionsCache.perms;
        }

        const payload = this.decodeJwtPayload<Record<string, any>>(token) ?? {};
        const result = new Set<string>();

        // 1) `permission` as array
        const permissionsArray = payload['permission'];
        if (Array.isArray(permissionsArray)) {
            permissionsArray.forEach((p: any) => {
                if (typeof p === 'string') result.add(p);
            });
        }

        // 2) `permission` as space-separated string
        if (typeof permissionsArray === 'string') {
            permissionsArray.split(' ')
                .map(x => x.trim())
                .filter(Boolean)
                .forEach(p => result.add(p));
        }

        // 3) multiple `permission` claims (handled by array check above, but for clarity)
        // Note: Some servers might send multiple "permission" claims which decode as array

        this.permissionsCache = { token, perms: result };
        return result;
    }

    /** Safe decode for JWT payload (no atob Unicode issues) */
    private decodeJwtPayload<T = any>(jwt: string): T | null {
        try {
            const payload = jwt.split('.')[1];
            if (!payload) return null;
            // base64url → base64
            const b64 = payload.replace(/-/g, '+').replace(/_/g, '/');
            const json = decodeURIComponent(
                atob(b64)
                    .split('')
                    .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                    .join('')
            );
            return JSON.parse(json) as T;
        } catch {
            return null;
        }
    }
}
