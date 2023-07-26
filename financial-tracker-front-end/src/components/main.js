"use client";
import React, { useState, useEffect } from "react";
import { useRouter, usePathname } from 'next/navigation'

import { Navigation } from '../components/navigation';

import  Head  from 'next/head'
import { userService } from '../services/user.service';


function Main({Component, pageProps}){
  const router = useRouter();
  const pathname = usePathname();
  const [user, setUser] = useState(null);
  const[authorized, setAuthorized] = useState(false);
  const [initial, setInitial] = useState(true)

 
  useEffect(() => {
    if(initial === true){
    //run initial authentication check on load
    
    // on route change start - hide content by setting authorization to false
    const hideContent = () => setAuthorized(false);
    hideContent();

    authCheck(pathname);
    
    
    

    // on route change complete - run auth check 
    

    // unsubscribe from events in useEffect return function
    return () => {
        setInitial(false)
    }

  }}, [pathname])


function authCheck(url) {
  // redirect to login page if accessing a private page and not logged in 
  setUser(userService.userValue);
  const publicPaths = ['/account/login', '/account/register'];
  const path = url.split('?')[0];
  if (!userService.userValue && !publicPaths.includes(path)) {
      setAuthorized(false);
      router.push('/account/login');
  } else {
      setAuthorized(true);
  }
}

return (
  <>
      <Head>
          <title>Financial Tracker</title>
          
          {/* eslint-disable-next-line @next/next/no-css-tags */}
          
      </Head>

      <div className={`app-container ${user ? 'bg-light' : ''}`}>
          <Navigation />
          {authorized &&
              <Component {...pageProps} />
          }
      </div>
  </>
);
}
export default Main;
