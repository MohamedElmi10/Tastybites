import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { ImageSrcPipe } from '../shared/image-src.pipe';
import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home.component';

@NgModule({
  declarations: [HomeComponent],
  imports: [SharedModule, HomeRoutingModule, ImageSrcPipe],
})
export class HomeModule {}
