/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  daisyui: {
    themes: [
      {
        pastel: {
          ...require("daisyui/src/theming/themes")["pastel"],
          "--rounded-box": "1rem", // border radius rounded-box utility class, used in card and other large boxes
          "--rounded-btn": "0.5rem", // border radius rounded-btn utility class, used in buttons and similar element
          "--rounded-badge": "1.9rem", // border radius rounded-badge utility class, used in badges and similar
          "--animation-btn": "0.25s", // duration of animation when you click on button
          "--animation-input": "0.2s", // duration of animation for inputs like checkbox, toggle, radio, etc
          "--btn-focus-scale": "0.95", // scale transform of button when you focus on it
          "--border-btn": "1px", // border width of buttons
          "--tab-border": "1px", // border width of tabs
          "--tab-radius": "0.5rem", // border radius of tabs
        },
      },
    ],
    prefix: "",
  },
  theme: {
    extend: {
      colors: {
        /*Week and calendar days*/
        'period-day':'#CB9292',
        'predicted-period-day':'#DBC2C6',
        'non-period-day':'#F9E8C8',
        'period-day-border':'#BA6E6E',
        'predicted-period-day-border':'#BA6E6E',
        'non-period-day-border':'#EEAF3A',
        /*Calendar*/
        'selected-day':'#edebed',
        'app-background':'#F6F3F7'
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
