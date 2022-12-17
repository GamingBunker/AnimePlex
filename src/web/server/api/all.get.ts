import {defineEventHandler} from "h3";
import axios from "axios";

const API_BASE = process.env.API_BASE_URL || 'http://localhost:5000';

export default defineEventHandler(async (event) => {
    const {data} = await axios.get(`${API_BASE}/all`);
    return data;
})