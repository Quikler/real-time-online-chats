/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        darkBlue: {
          100: "#00224D",
          200: "#0C1844",
        },
        lightGreen: {
          100: "#219C90",
          200: "#188277",
        },
        maroon: {
          100: "#70134f",
          200: "#5D0E41",
        },
        purple: {
          100: "#4335A7",
          200: "#392c99",
        }
      }
    },
  },
  plugins: [],
}
