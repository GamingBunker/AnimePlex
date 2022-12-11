import {defineEventHandler, getQuery} from "h3";
import axios from "axios";

const API_BASE = process.env.API_BASE_URL;

export default defineEventHandler(async (event) => {
    const {url} = getQuery(event)
    const {data} = await axios.post(`${API_BASE}/manga/download`, {url});
    return data;
})