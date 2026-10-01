import { type CSSProperties, type ReactNode } from 'react';

import '../../styles/components/ui/hero.css';

export default function Hero( { bgUrl, children } : { bgUrl: string, children: ReactNode} ) {

  return (
    <section
      className="hero"
      style={{'--hero-bg': `url(${bgUrl})`} as CSSProperties}>
      {children}
    </section>
  );
}