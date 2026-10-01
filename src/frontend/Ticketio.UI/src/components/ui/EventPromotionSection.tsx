import '../../styles/components/ui/promotion.css';

export default function EventPromotionSection() {

  return (
    <>
      <div className="promotion-grid">
        <div className="promotion-left-section">
          <div className="promo-main-item">
            <img src='https://images.pexels.com/photos/3656773/pexels-photo-3656773.jpeg' />
            <p>Headline</p>
            <h3>Event name</h3>
          </div>
        </div>
        <div className="promotion-right-section">
          <div className="promo-item">
            <img src="https://images.pexels.com/photos/3656773/pexels-photo-3656773.jpeg" alt="Promo 1" />
            <p>Headline</p>
            <h4>Event name</h4>
          </div>

          <div className="promo-item">
            <img src="https://images.pexels.com/photos/3656773/pexels-photo-3656773.jpeg" alt="Promo 1" />
            <p>Headline</p>
            <h4>Event name</h4>
          </div>

          <div className="promo-item">
            <img src="https://images.pexels.com/photos/3656773/pexels-photo-3656773.jpeg" alt="Promo 1" />
            <p>Headline</p>
            <h4>Event name</h4>
          </div>

          <div className="promo-item">
            <img src="https://images.pexels.com/photos/3656773/pexels-photo-3656773.jpeg" alt="Promo 1" />
            <p>Headline</p>
            <h4>Event name</h4>
          </div>
        </div>
      </div>
    </>
  );
}