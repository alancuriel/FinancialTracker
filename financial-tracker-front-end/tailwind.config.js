/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/pages/**/*.{js,ts,jsx,tsx,mdx}',
    './src/components/**/*.{js,ts,jsx,tsx,mdx}',
    './src/app/**/*.{js,ts,jsx,tsx,mdx}',
  ],
  theme: {

    extend: {

      colors: {
        gunmetaldark: "#202C39",
        gunmetallight: "#283845",
        sage: "#B8B08D",
        peach: "#F2D492",
        tangerine: "#F29559",
        gunmetallighter:"#385368",
        white: "#FFF"
    },
      backgroundImage: {
        'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
        'gradient-conic':
          'conic-gradient(from 180deg at 50% 50%, var(--tw-gradient-stops))',
      },
      animation: {
        blob: "blob 7s infinite"          
        },
      keyframes: {
        blob: {
          "0%":{
            transform: "scale(1)"
          },
          "33%":{
            transform: "scale(1.1)"
          },
          "66%":{
            transform: "scale(0.9)"
          },
          "100%":{
            transform: "scale(1)"
          }

        }
      }
    },
  },
  plugins: [
    require('@tailwindcss/forms')
  ],
}
