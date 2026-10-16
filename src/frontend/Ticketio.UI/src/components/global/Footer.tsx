import logo from '../../assets/images/logo_white.png';

import '../../styles/components/footer.css';

export default function Footer() {
  return (
    <footer className="footer">
      <div className="footer-container">

        <div className="footer-top">
          <div className="footer-brand">
            <img src={logo} className="footer-logo" alt="Ticketio Logo" />
            <p className="footer-tagline">
              Discover events. Experience more.
            </p>
          </div>

          <div className="footer-links">
            <div className="footer-link-group">
              <h3>Explore</h3>
              <ul>
                <li><a href="/">Home</a></li>
                <li><a href="/events">Browse Events</a></li>
              </ul>
            </div>

            <div className="footer-link-group">
              <h3>Account</h3>
              <ul>
                <li><a href="/login">Login</a></li>
                <li><a href="/register">Create Account</a></li>
              </ul>
            </div>

            <div className="footer-link-group">
              <h3>Legal</h3>
              <ul>
                <li><a href="/purchase-policy">Purchase Policy</a></li>
                <li><a href="/privacy-policy">Privacy Policy</a></li>
                <li><a href="/cookies">Cookies</a></li>
              </ul>
            </div>
          </div>
        </div>

        <div className="footer-disclaimer">
          <p>Ticketio is a development and portfolio project, not a real ticketing service. All events, artists and content displayed are fictional.</p>
        </div>

        <div className="footer-bottom">
          <p>Ticketio. Developed by JO.</p>
          <p>Built for demonstration purposes.</p>
        </div>

      </div>
    </footer>
  );
}