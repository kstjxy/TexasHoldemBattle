M = {
    name = 'nono005',
    myaction = -1;
}

function M: startfunction(gamerule)    --游戏最开始(初始化，等待玩家响应)  
    print(M['name'] , "初始化成功！")
    return M['name']
end

function M: round_start(gamestat)    --每【场】开始(仅返回每个玩家开局时情况)
    print( M['name'] , "开局！") 
    return
end

function M: action(gamestat)    --每【轮】调用动作(返回详细信息，等待玩家操作)
    ranNum = gamestat.NumRandom
    if (ranNum <= 50) then
        M.myaction = 1
        return M.myaction
    elseif (ranNum <= 90) then
        M.myaction = 2
        return M.myaction
    elseif (ranNum <= 98) then
        M.myaction = 3
        return M.myaction
    end
    M.myaction = 4
    return M.myaction
end

function M: finalCards(gamestat)    --每【轮】调用动作(返回详细信息，等待玩家操作)
    tb = {}
    math.randomseed(tostring(os.time()):reverse():sub(1, 7))
    while #tb < 5 do 
		local istrue = false
		local num = math.random(0,6)
		if #tb ~= nil then
			for i = 1 ,#tb do
				if tb[i] == num then
					istrue = true
				end
			end
		end
		if istrue == false then
			table.insert( tb, num )
		end
	end
    list = {}
    for i,v in ipairs(tb) do
        if (v<2) then
            list[i] = gamestat.CardsInHands[v]
        else
            list[i] = gamestat.CommunityCards[v-2]
        end
    end

    return list
end