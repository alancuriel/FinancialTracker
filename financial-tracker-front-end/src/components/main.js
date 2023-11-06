"use client";
import React, { useState, useEffect, useRef } from "react";
import { useRouter, usePathname, redirect } from 'next/navigation'
import  Head  from 'next/head'
import { userService } from '@/services/user.service';
import Transactions from  '@/components/v0/transactions'


function Main({Component, pageProps}){
  const router = useRouter();
  const pathname = usePathname();
  const [user, setUser] = useState(null);
  const[authorized, setAuthorized] = useState(false);
  const initial = useRef(true)

 
  useEffect(() => {
    if(initial.current === true){
      console.log("hook")
    //run initial authentication check on load
    
    // on route change start - hide content by setting authorization to false
    const hideContent = () => setAuthorized(false);
    hideContent();

    authCheck(pathname);
    

    // on route change complete - run auth check 
    userService.user.subscribe( u => {
      authCheck(pathname)
    })

    

    // unsubscribe from events in useEffect return function
    return () => {
        initial.current = false
    }

  }}, [])


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
      </Head>

      <div className='app-container w-full'>
          {authorized && <Transactions/>}
      </div>
  </>
);
}
export default Main;
