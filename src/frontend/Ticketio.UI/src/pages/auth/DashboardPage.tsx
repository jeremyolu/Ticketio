import { Link } from 'react-router-dom';

import '../../styles/pages/dashboard.css';

const user = {
  name: 'Alex Johnson',
  email: 'alex@example.com',
  memberSince: 'March 2025',
};

const stats = [
  { label: 'Upcoming events', value: 3 },
  { label: 'Tickets purchased', value: 12 },
  { label: 'Saved events', value: 7 },
];

const orders = [
  { id: 'TK-10482', event: 'Summer Rooftop Party', date: '14 Nov 2026', status: 'Confirmed' },
  { id: 'TK-10377', event: 'Live Jazz Night', date: '28 Nov 2026', status: 'Confirmed' },
  { id: 'TK-10112', event: 'Comedy Showcase', date: '02 Oct 2026', status: 'Attended' },
  { id: 'TK-09954', event: 'Food & Drink Festival', date: '19 Sep 2026', status: 'Cancelled' },
];

function initials(name: string) {
  return name
    .split(' ')
    .map((part) => part[0])
    .join('')
    .slice(0, 2)
    .toUpperCase();
}

export default function DashboardPage() {
  const handleSignOut = () => {
    console.log('sign out');
  };

  return (
    <div className="dash">
      <header className="dash-header">
        <div className="dash-welcome">
          <h1 className="dash-title">Welcome back, {user.name.split(' ')[0]}</h1>
          <p className="dash-subtitle">Discover events. Experience more...</p>
        </div>
        <Link to="/search" className="button">
          Browse events
        </Link>
      </header>

      <section className="dash-stats">
        {stats.map((stat) => (
          <div className="dash-stat" key={stat.label}>
            <span className="dash-stat-value">{stat.value}</span>
            <span className="dash-stat-label">{stat.label}</span>
          </div>
        ))}
      </section>

      <div className="dash-grid">
        <section className="dash-card">
          <div className="dash-card-header">
            <h2 className="dash-card-title">Recent orders</h2>
            <Link to="/orders" className="dash-link">
              View all
            </Link>
          </div>

          <div className="dash-table-wrap">
            <table className="dash-table">
              <thead>
                <tr>
                  <th>Order</th>
                  <th>Event</th>
                  <th>Date</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {orders.map((order) => (
                  <tr key={order.id}>
                    <td className="dash-order-id">{order.id}</td>
                    <td>{order.event}</td>
                    <td>{order.date}</td>
                    <td>
                      <span className={`dash-badge dash-badge-${order.status.toLowerCase()}`}>
                        {order.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>

        <aside className="dash-side">
          <section className="dash-card">
            <div className="dash-profile">
              <div className="dash-avatar">{initials(user.name)}</div>
              <div className="dash-profile-info">
                <span className="dash-profile-name">{user.name}</span>
                <span className="dash-profile-email">{user.email}</span>
              </div>
            </div>
            <p className="dash-member">Member since {user.memberSince}</p>
            <Link to="/account/settings" className="dash-button-outline">
              Edit profile
            </Link>
          </section>

          <section className="dash-card">
            <h2 className="dash-card-title">Quick links</h2>
            <ul className="dash-links">
              <li>
                <Link to="/orders" className="dash-quick">
                  My tickets
                </Link>
              </li>
              <li>
                <Link to="/saved" className="dash-quick">
                  Saved events
                </Link>
              </li>
              <li>
                <Link to="/account/settings" className="dash-quick">
                  Account settings
                </Link>
              </li>
            </ul>
            <button className="dash-signout" type="button" onClick={handleSignOut}>
              Sign out
            </button>
          </section>
        </aside>
      </div>
    </div>
  );
}