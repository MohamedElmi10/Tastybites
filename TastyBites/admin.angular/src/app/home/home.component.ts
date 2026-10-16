import { AuthService, PermissionService } from '@abp/ng.core';
import { Component, OnInit, inject } from '@angular/core';
import { Observable, forkJoin } from 'rxjs';
import { RecipeAdminService, RecipeDto, RecipeStatsDto } from '@proxy/recipes';
import { publicSiteUrl } from '../../environments/environment';

const RECENT_COUNT = 5;

interface DashboardData {
  stats: RecipeStatsDto;
  recent: RecipeDto[];
}

// Admin landing page: a login card when signed out, a small recipe dashboard when signed in.
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly permissionService = inject(PermissionService);
  private readonly recipeAdminService = inject(RecipeAdminService);

  readonly publicSiteUrl = publicSiteUrl;

  // Stays undefined when the user may not manage recipes; the template then shows a short note instead
  dashboard$?: Observable<DashboardData>;

  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated;
  }

  ngOnInit(): void {
    // why: only call the admin API when the user has the permission - otherwise it answers 403
    // and ABP shows an error popup on the landing page
    if (this.hasLoggedIn && this.permissionService.getGrantedPolicy('TastyBites.Recipes')) {
      // forkJoin = run both requests in parallel and continue when both are done
      this.dashboard$ = forkJoin({
        stats: this.recipeAdminService.getStats(),
        recent: this.recipeAdminService.getRecent(RECENT_COUNT),
      });
    }
  }

  login(): void {
    this.authService.navigateToLogin();
  }
}
