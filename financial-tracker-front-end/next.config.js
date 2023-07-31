/** @type {import('next').NextConfig} */
module.exports = {

    compiler: {     styledComponents: true   },
    experimental: {
        appDir: true
      },
    
    reactStrictMode: true,
    serverRuntimeConfig: {
        secret: 'THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET, IT CAN BE ANY STRING'
    },
    publicRuntimeConfig: {
        apiUrl: process.env.NODE_ENV === 'development'
            ? 'http://localhost:3000/api' // development api
            : 'http://localhost:3000/api' // production api
    },
    env: {
        NEXT_PUBLIC_PUBLICAPI: 'https://localhost:7107'
    }
}