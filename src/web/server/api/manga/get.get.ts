import {defineEventHandler, getQuery} from "h3";
import axios from "axios";

const API_BASE = process.env.API_BASE_URL_BASE;

export default defineEventHandler(async (event) => {
    const {search} = getQuery(event)
    const {data} = await axios.get(`${API_BASE}/manga/name/${search}`);
    return data;
})