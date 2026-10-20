import { Outlet } from 'react-router-dom';

import Nav from '../components/global/Nav';
import Footer from '../components/global/Footer';

export default function MainLayout() {
  return (
    <>
      <Nav />
      <main>
        <Outlet />
      </main>
      <Footer />
    </>
  );
}