import { FaBullhorn, FaCompass, FaEnvelope, FaLocationArrow, FaMusic, FaTheaterMasks, FaUmbrellaBeach, FaUser } from 'react-icons/fa';
import { FaTicket } from 'react-icons/fa6';
import logo from '../../assets/images/logo.png';

import '../../styles/components/nav.css';

const iconSize = 13;
const topNaviconColour = 'white';
const bottomNaviconColour = 'black';

export default function Nav() {

  const placeholders = [
    'search artists, comedians or musicians...',
    'search concerts, shows or festivals...',
    'search cities, venues or locations...'
  ];

  const randomPlaceholderText = placeholders[Math.floor(Math.random() * placeholders.length)];

  return (
    <nav className="navbar">
      <div className="navbar-top">
        <div className="navbar-top-left">
          <p><FaLocationArrow size={iconSize} color={topNaviconColour} className="mr-10" />Location - UK</p>
        </div>
        <div className="navbar-top-right">
          <ul className="navbar-menu-items">
            <li><FaTicket  size={iconSize} color={topNaviconColour} className="mr-10" />Tickets</li>
            <li><FaBullhorn  size={iconSize} color={topNaviconColour} className="mr-10" />Promoters</li>
            <li><FaEnvelope  size={iconSize} color={topNaviconColour} className="mr-10" />Contact</li>
            <li><FaUser size={iconSize} color={topNaviconColour} className="mr-10" />Login/Register</li>
          </ul>
        </div>
      </div>

      <div className="navbar-bottom">
        <div className="navbar-bottom-left">
          <img src={logo} className="navbar-logo" alt="Ticketio Logo" />
          <ul className="navbar-menu-items">
            <li><FaMusic size={iconSize} color={bottomNaviconColour} className="mr-10" />Concerts</li>
            <li><FaTheaterMasks size={iconSize} color={bottomNaviconColour} className="mr-10" />Shows</li>
            <li><FaUmbrellaBeach size={iconSize} color={bottomNaviconColour} className="mr-10" />Festivals</li>
            <li><FaCompass size={iconSize} color={bottomNaviconColour} className="mr-10" />Explore</li>
          </ul>
        </div>
        <div className="navbar-bottom-right">
          <div className="navbar-search">
            <input type="text" className="form-control" placeholder={randomPlaceholderText} />
            <button type="submit" className="button">Search</button>
          </div>
        </div>
      </div>
    </nav>
  );
}