"use client";
import React, { useState, useEffect } from "react";
import { useRouter, usePathname } from 'next/navigation'

import { Navigation } from '../components/navigation';

import  Head  from 'next/head'
import { userService } from '../services/user.service';
import Transactions from  '~/components/v0/transactions'


function Main({Component, pageProps}){
  const router = useRouter();
  const pathname = usePathname();
  // const [user, setUser] = useState(null);
  const authorized = useState(false);
  // const [initial, setInitial] = useState(true)

 
  useEffect(() => {
    console.log("main wow")
    //run initial authentication check on load
    
    // on route change start - hide content by setting authorization to false
    // const hideContent = () => setAuthorized(false);
    // hideContent();

    authCheck(pathname);
    

    // on route change complete - run auth check 
    

    // unsubscribe from events in useEffect return function

  }, [])


function authCheck(url) {
  console.log("YESSIR")
  // redirect to login page if accessing a private page and not logged in 
  const publicPaths = ['/account/login', '/account/register'];
  const path = url.split('?')[0];
  
  if (!publicPaths.includes(path) && !userService.userValue) {

      if (authorized) {
        setAuthorized(false);
      }
      router.push('/account/login');
  } else {
    if (!authorized) {
      setAuthorized(true);
    }
  }
}

return (
  <>
      <Head>
          <title>Financial Tracker</title>
      </Head>

      <div className='app-container w-full'>
          {authorized && <Transactions/>}
      </div>
  </>
);
}
export default Main;
