import { TestBed } from '@angular/core/testing';
import { App } from './app';

describe('App shell', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [App] }).compileComponents();
  });

  it('shows the product name', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const root = fixture.nativeElement as HTMLElement;
    expect(root.querySelector('h1')?.textContent).toContain('MontrealFoodViolations');
  });

  it('shows the dataset attribution in the footer', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const footer = (fixture.nativeElement as HTMLElement).querySelector('footer');
    expect(footer?.textContent).toContain('Ville de Montréal');
    expect(footer?.querySelector('a')?.getAttribute('href')).toContain('donnees.montreal.ca');
  });
});
