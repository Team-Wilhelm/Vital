/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        'background' : '#EFEAE6',
        'yellow-accent': '#EEAF3A',
        'green-accent' : '#82B09E',
        'card' : '#FAF7F5',
        'card-hover' : '#ffffff',
      },
    },
    container: {
      center: true,
      padding: '1rem',
    }
  },
  plugins: [require("daisyui")],
}

/*
 *  DEFAULT: '1rem',
 *         sm: '2rem',
 *         lg: '4rem',
 *         xl: '5rem',
 *         '2xl': '6rem',
 */
