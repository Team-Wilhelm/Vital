/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  daisyui: {
    themes: ["cupcake"],
  },
  theme: {
    extend: {
      colors: {
        'background' : '#EFEAE6',
        'yellow-accent': '#EEAF3A',
        'green-accent' : '#82B09E',
        'green-light-accent' : '#B8DACD',
        'card' : '#FAF7F5',
        'card-hover' : '#ffffff',
        'button-selected': '#EEAF3A',
        'button-unselected': '#B8DACD',
      },
    },
    container: {
      center: true,
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
