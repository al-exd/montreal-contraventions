import { expect, test } from '@playwright/test';

test('home page shows the dataset attribution', async ({ page }) => {
  await page.goto('/');
  await expect(page).toHaveTitle('MontrealFoodViolations');
  await expect(page.getByRole('heading', { name: 'MontrealFoodViolations' })).toBeVisible();
  await expect(page.getByRole('contentinfo')).toContainText('Ville de Montréal');
});
