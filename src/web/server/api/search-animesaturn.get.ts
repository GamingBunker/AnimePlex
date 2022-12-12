import {defineEventHandler, getQuery} from "h3";
import axios from "axios";

const API_BASE = process.env.API_BASE_URL;

export default defineEventHandler(async (event) => {
    const {search} = getQuery(event)
    const {data} = await axios.get(`${API_BASE}/anime/list/name/${search}`);
    return data;
})