import {Component, OnInit} from '@angular/core';
import {Router, RouterLink} from "@angular/router";
import {routes} from "../../../routes/routes";
import {AuthService} from "../../../core/auth/auth.service";
import {TranslatePipe} from "@ngx-translate/core";
import {NgIf} from '@angular/common';
import {TokenService} from '../../../core/auth/token.service';

@Component({
  selector: 'app-access-denied',
  templateUrl: './access-denied.component.html',
  styleUrl: './access-denied.component.scss',
  imports: [
    TranslatePipe,
    RouterLink,
    NgIf
  ],
  standalone: true
})
export class AccessDeniedComponent implements OnInit{
  public routes = routes;
  public previousUrl: string = '/';

  constructor(
    protected authService: AuthService,
    protected tokenService: TokenService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Get the previous URL from state or default home
    this.previousUrl = this.router.getCurrentNavigation()?.extras?.state?.['previousUrl'] || '/';
  }
}
