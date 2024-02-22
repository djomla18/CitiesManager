import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CitiesComponent } from './cities/cities.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CitiesComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
}
