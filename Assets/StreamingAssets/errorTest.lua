M = {}

M.name = 'test.error'
M.myaction = -1


function M: action(gamestat)    --ÿ���֡����ö���(������ϸ��Ϣ���ȴ���Ҳ���)
    math.randomseed(tostring(os.time()):reverse():sub(1, 7))
    ranNum = math.random(100)
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

function M: finalCards(gamestat)    --ÿ���֡����ö���(������ϸ��Ϣ���ȴ���Ҳ���)
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