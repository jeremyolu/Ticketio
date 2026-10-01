import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import type Event from '../models/Event';

import heroBg from '../assets/images/navbar-bg.jpg';
import Hero from '../components/ui/Hero';
import EventPromotionSection from '../components/ui/EventPromotionSection';

import '../styles/pages/home.css';

export default function HomePage() {

  const navigate = useNavigate();

  const [searchQuery, setSearchQuery] = useState('');

  const handleSearch = () => {
    if (searchQuery.trim()) {
      navigate(`/search?q=${encodeURIComponent(searchQuery.trim())}`);
    }
  };

  const events: Event[] = [
    { eventId: 1, title: "Event 1", city: "London", startDate: "2028-09-12" },
    { eventId: 2, title: "Event 2", city: "Manchester", startDate: "2028-09-18" },
    { eventId: 3, title: "Event 3", city: "Liverpool", startDate: "2028-09-23" },
    { eventId: 4, title: "Event 4", city: "London", startDate: "2028-09-28" },
    { eventId: 5, title: "Event 5", city: "Cardiff", startDate: "2028-09-30" }
  ];

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString("en-GB", {
      day: "numeric",
      month: "short",
      year: "numeric"
    });
  }

  return (
    <>
      <Hero bgUrl={heroBg}>
        <div className="search-content">
          <h1 className="search-title">Search for your next experience here.</h1>
          <div className="search-search">
            <input type="text" className="form-control" 
              placeholder="Search for concerts, shows, festivals..." 
              value={searchQuery} 
              onChange={(e) => setSearchQuery(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  handleSearch();
                }
              }}
              />
            <button type="submit" 
              className="button" 
              onClick={handleSearch}>Explore</button>
          </div>
        </div>
      </Hero>

      <section className= "promotion-section">
        <EventPromotionSection />
      </section>
      
      <section className="featured-section">
      </section>
    </>
  );
}