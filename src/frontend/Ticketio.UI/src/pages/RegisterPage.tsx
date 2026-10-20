import { useState } from 'react';

import '../styles/pages/auth.css';

export default function RegisterPage() {

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [remember, setRemember] = useState(false);

  const handleSubmit = (e: any) => {
    e.preventDefault();
    console.log({ email, password, remember });
  };

  return (
    <div className="auth-page">
      <main className="auth-card">
        <header className="auth-header">
          <h1 className="auth-title">Register Now</h1>
          <p className="auth-subtitle">Create your account to continue</p>
        </header>

        <form className="auth-form" onSubmit={handleSubmit}>
          <div className="auth-field">
            <label className="auth-label" htmlFor="email">
              Email
            </label>
            <input
              id="email"
              className="auth-input"
              type="email"
              placeholder="you@example.com"
              autoComplete="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="auth-field">
            <label className="auth-label" htmlFor="password">
              Password
            </label>
            <input
              id="password"
              className="auth-input"
              type="password"
              placeholder="Enter your password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          <div className="auth-options">
            <label className="auth-remember">
              <input
                type="checkbox"
                checked={remember}
                onChange={(e) => setRemember(e.target.checked)}
              />
              <span>Remember me</span>
            </label>
          </div>

          <button type="submit" className="button">Register</button>
        </form>

        <p className="auth-footer">
          Have an account?{' '}
          <a className="auth-link" href="/login">
            Login
          </a>
        </p>
      </main>
    </div>
  );
}
