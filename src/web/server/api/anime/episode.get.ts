import {defineEventHandler, getQuery} from "h3";
import axios from "axios";

const API_BASE = process.env.API_BASE_URL || 'http://localhost:5000';

export default defineEventHandler(async (event) => {
    const {name} = getQuery(event)
    const {data} = await axios.get(`${API_BASE}/episode/name/${name}`);
    return data;
})