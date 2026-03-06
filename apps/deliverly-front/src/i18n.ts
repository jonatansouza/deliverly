import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import { TRANSLATE_RESOURCES } from "./translates.i18n";

i18n.use(initReactI18next).init({
  resources: TRANSLATE_RESOURCES,
  lng: "pt",
  fallbackLng: "en",
  interpolation: { escapeValue: false },
});

export default i18n;
