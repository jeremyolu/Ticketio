import { Link, Outlet } from 'react-router-dom';

import '../styles/global/auth-layout.css';

export default function AuthLayout() {
  return (
    <div className="auth-layout">
      <Link to="/" className="auth-layout-brand">
        Ticketio
      </Link>

      <div className="auth-layout-content">
        <Outlet />
      </div>

      <footer className="auth-layout-footer">
        &copy; {new Date().getFullYear()} Ticketio. All rights reserved.
      </footer>
    </div>
  );
}
