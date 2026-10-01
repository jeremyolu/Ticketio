import '../../styles/components/footer.css';

export default function Footer() {

  return (
    <footer className="footer">
      <div className="footer-top">
       
      </div>

      <div className="footer-bottom">
        <div className="footer-disclaimer">
          <p className="footer-disclaimer-text">Note: This is a development/portfolio project and is not a real ticketing service.</p>
          <p className="footer-disclaimer-text">All events, artists and content shown are entirely fictional and should not be considered legitimate.</p>
        </div>
        <div className="legal-section">
          <ul className="legal-menu-items">
            <li>Purchase Policy</li><span className="separator">|</span>
            <li>Privacy Policy</li><span className="separator">|</span>
            <li>Cookies</li><span className="separator">|</span>
            <li>Manage Cookies</li>
          </ul>
           <p>2026 Ticketio. Developed by JO</p>
        </div>
      </div>
    </footer>
  );
}