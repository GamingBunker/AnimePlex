import {defineEventHandler, readBody} from "h3";
import axios from "axios";

const API_BASE = process.env.API_BASE_URL;

export default defineEventHandler(async (event) => {
    const body = await readBody(event);
    const {data} = await axios.put(`${API_BASE}/anime/redownload`, body.media);
    return data;
})