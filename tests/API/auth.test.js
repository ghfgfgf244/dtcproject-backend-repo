const { test, expect } = require('@playwright/test');

test.describe('Auth API Flow', () => {
  const email = `test_${Date.now()}@example.com`;
  const password = 'Password123!';

  test('should register a new user', async ({ request }) => {
    const response = await request.post('/api/auth/register', {
      data: {
        email,
        password,
        fullName: 'Playwright Test User',
        phone: '0987654321'
      }
    });

    expect(response.ok()).toBeTruthy();
    const body = await response.json();
    expect(body.success).toBe(true);
    expect(body.message).toContain('registered');
  });

  test('should login successfully and return a token', async ({ request }) => {
    const response = await request.post('/api/auth/login', {
      data: {
        email,
        password
      }
    });

    expect(response.ok()).toBeTruthy();
    const body = await response.json();
    expect(body.success).toBe(true);
    expect(body.data).toHaveProperty('token');
    
    const token = body.data.token;
    
    // Verify token works on profile
    const profileResponse = await request.get('/api/users/me', {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });
    expect(profileResponse.ok()).toBeTruthy();
    const profileBody = await profileResponse.json();
    expect(profileBody.success).toBe(true);
    expect(profileBody.data.email).toBe(email);
  });
});
